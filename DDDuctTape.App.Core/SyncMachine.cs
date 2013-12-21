using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace DDDuctTape.App.Core
{
    public class SyncMachine
    {
        protected IUnityContainer IoC { get; set; }

        public SyncMachine(IUnityContainer ioc)
        {
            IoC = ioc;
        }

        public async Task<int> CopyMissingFilesNoOverWrite(string source, string destination, IProgress<MaintenanceProgress> progress, CancellationToken ct)
        {
            string[] originalFiles = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
            int totalFiles = originalFiles.Count();
            int filesCopied = await Task.Run<int>(
                () =>
                {
                    int justProcessed = 0;
                    int reallyCopied = 0;
                    Array.ForEach(originalFiles, (originalFileLocation) =>
                    {
                        FileInfo originalFile = new FileInfo(originalFileLocation);
                        FileInfo destFile = new FileInfo(originalFileLocation.Replace(source, destination));
                        var exceptions = new List<string>();
                        var bads = new List<string>();
                        if (!destFile.Exists)
                        {
                            int retryCount = 0;
                            bool tryAgain = false;
                            bads.Add(destFile.FullName);
                            do
                            {
                                ct.ThrowIfCancellationRequested();
                                try
                                {
                                    Directory.CreateDirectory(destFile.DirectoryName);
                                    originalFile.CopyTo(destFile.FullName, false);
                                    reallyCopied++;
                                }
                                catch (Exception e)
                                {
                                    tryAgain = true;
                                    retryCount++;
                                    if (retryCount > 12)
                                    {
                                        tryAgain = false;
                                        exceptions.Add(destFile.FullName);
                                    }
                                    else
                                    {
                                        Thread.Sleep(250);
                                    }
                                }
                            } while (tryAgain);
                        }
                        justProcessed++;
                        if (progress != null)
                        {
                            progress.Report(new MaintenanceProgress()
                            {
                                Progress = justProcessed * 100 / totalFiles,
                                Errors = exceptions,
                                Bads = bads,
                                QueueLength = totalFiles,
                                QueueComplete = justProcessed
                            });
                        }
                    });
                    return reallyCopied;
                }, ct);
            return filesCopied;
        }

        public async Task<int> Maintenance9(string source, string destination, IProgress<MaintenanceProgress> progress, CancellationToken ct)
        {
            /*
             a.	считаем все файлы с расширением CDX в корневом каталоге INITDATA (в подкаталоги не смотрим), считаем все файлы CDX в корневом каталоге SITEDATA, сравниваем, 
                i.	если в корне INITDATA есть CDX-файлы, которых нет в корне SITEDATA, то указываем эти файлы красной записью в итогах (в пункте 19). Выполняем пункт b.
                ii.	Если все файлы в SITEDATA уже есть (или их даже больше, чем есть в INITDATA, что нормально), то красную запись не делаем, а выполняем следующее: 
             b.	копируем ВСЕ CDX-файлы из корневого каталога INIDATA в корневой каталог SITEDATA, перезаписывая все имеющиеся там (маска “*.CDX”);
             */
            return await PerformMaintenanceByMask(source, destination, "*.cdx", progress, ct);
        }

        public async Task<int> Maintenance11(string source, string destination, IProgress<MaintenanceProgress> progress, CancellationToken ct)
        {
            /*
            a.	считаем все файлы, чьи имена заканчиваются на WK, в корневом каталоге INITDATA, считаем все файлы, заканчивающиеся на WK, в корневом каталоге SITEDATA, сравниваем,
            i.	если в корне INITDATA есть *WK-файлы, которых нет в корне SITEDATA, то указываем эти файлы красной записью в итогах (в пункте 19). Выполняем пункт b.
            ii.	Если все файлы в SITEDATA уже есть (или их даже больше, чем есть в INITDATA, что нормально), то красную запись не делаем, а делаем следующее:
            b.	Копируем все файлы, заканчивающиеся на WK, из корневого каталога INITDATA в корневой каталог SITEDATA, перезаписывая все имеющиеся там (маска “*WK.*”);
             */
            return await PerformMaintenanceByMask(source, destination, "*WK.*", progress, ct);
        }

        public async Task<int> Maintenance12(string source, string destination, IProgress<MaintenanceProgress> progress, CancellationToken ct)
        {
            /*
            a.	считаем все RPT-файлы в каталоге Applications в пути 1 (CD\Apps), считаем все RPT-файлы в каталоге Applications в пути 4 (DD\Apps), сравниваем, 
                i.	если в пути 1 есть RPT-файлы, которых нет в пути 4, то указываем эти файлы красной записью в итогах (в пункте 19). Выполняем пункт b.
                ii.	Если все файлы в пути 4 уже есть (или их даже больше, чем есть в пути 1), то красную запись не делаем, а делаем следующее: 
            b.	копируем все RPT-файлы из пути 1 (CD\Apps) в путь 4 (DD\Apps), перезаписывая все имеющиеся там (маска “*.RPT”);
            */
            return await PerformMaintenanceByMask(source, destination, "*.RPT", progress, ct);
        }

        public async Task<int> Maintenance13(string source, string destination, IProgress<MaintenanceProgress> progress, CancellationToken ct)
        {
            /*
             13.	Педаль 13 - копируем все файлы по маске “??MMDDYYYY.*” из подкаталогов INITDATA в подкаталоги SITEDATA, перезаписывая все имеющиеся там.
             * КРИТИЧНО - раскладывая их по тем же каталогам, откуда они копируются. Полностью сохраняем структуру подкаталогов.
             * Она, скорее всего, будет в 1-2 уровня глубиной, но папок будут десятки; названия должны быть точно скопированы В ВЕРХНЕМ РЕГИСТРЕ из исходника.
             * Если в исходнике нижний регистр – меняем на верхний при копировании;
             */

            var reg = new Regex(@"\\.{2}(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])(19|20)\d\d\..*$"); // ??MMDDYYYY.*

            int files = await Task.Run(
                () =>
                {
                    var exceptions = new List<string>();
                    var originalFiles = new List<string>();

                    try
                    {
                        ct.ThrowIfCancellationRequested();
                        originalFiles = (from file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories) where reg.IsMatch(file) select file).ToList();
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e.ToString());
                        return 0;
                    }

                    var bads = new List<string>();
                    int totalFiles = originalFiles.Count();
                    int justProcessed = 0;
                    foreach (var originalFileLocation in originalFiles)
                    {
                        var originalFile = new FileInfo(originalFileLocation);
                        var destFile = new FileInfo(originalFileLocation.Replace(source, destination));
                        bool tryAgain = false;
                        int retryCount = 0;
                        bads.Add(destFile.FullName);
                        do
                        {
                            ct.ThrowIfCancellationRequested();
                            try
                            {
                                if (!Directory.Exists(destFile.DirectoryName))
                                {
                                    Directory.CreateDirectory(destFile.DirectoryName.ToUpper());
                                }
                                if (destFile.IsReadOnly)
                                {
                                    destFile.IsReadOnly = false;
                                }
                                originalFile.CopyTo(destFile.FullName, true);
                            }
                            catch (Exception e)
                            {
                                tryAgain = true;
                                retryCount++;
                                if (retryCount > 12)
                                {
                                    tryAgain = false;
                                    exceptions.Add(destFile.FullName);
                                }
                                else
                                {
                                    Thread.Sleep(250);
                                }
                            }
                        } while (tryAgain);
                        justProcessed++;
                        if (progress != null)
                        {
                            progress.Report(new MaintenanceProgress()
                            {
                                Progress = justProcessed * 100 / totalFiles,
                                Errors = exceptions,
                                Bads = bads,
                                QueueLength = totalFiles,
                                QueueComplete = justProcessed
                            });
                        }
                    }

                    return justProcessed;
                },
                ct
            );
            return files;
        }

        public async Task<IList<string>> Maintenance13b(string source, IProgress<MaintenanceProgress> progress, CancellationToken ct)
        {
            /*
             * педалька 13B – делаем проверку на наличие файлов с нулевым размером в пути 5 (SITEDATA) включая подкаталоги.
             * При проверке используем следующий фильтр: игнорируем текстовые файлы (*.TXT), а также файлы, начинающиеся с DDTEMP (DDTEMP*.*).
             * Если файлы по данному фильтру с 0 размером таки обнаруживаются, то пишем имена этих счастливчиков с полным путем в пункт 19 красным колером.
            */
            //var pattern = new WildcardPattern("DDTEMP*.*");
            var pattern = new Regex(@"\DDTEMP.*\..*$", RegexOptions.IgnoreCase);

            IList<string> files = await Task.Run(
                () =>
                {
                    var exceptions = new List<string>();
                    var badFiles = new List<string>();

                    try
                    {
                        ct.ThrowIfCancellationRequested();
                        badFiles = (from file in Directory.EnumerateFiles(source, "*.*", SearchOption.AllDirectories)
                                    where
                                        Path.GetExtension(file) != ".txt" && Path.GetExtension(file) != ".TXT" &&
                                        !pattern.IsMatch(file) &&
                                        (new FileInfo(file)).Length == 0
                                    select file).ToList();
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e.ToString());
                    }

                    if (progress != null)
                    {
                        progress.Report(new MaintenanceProgress()
                        {
                            Progress = 100,
                            Errors = exceptions,
                            Bads = badFiles,
                            QueueLength = badFiles.Count,
                            QueueComplete = badFiles.Count
                        });
                    }

                    return badFiles;
                },
                ct
            );
            return files;
        }

        public void RepairStart(string source)
        {
            System.Diagnostics.Process.Start(source + @"\DDWIN.EXE", "/REPAIR_ALL");
        }

        public async Task<int> PerformMaintenanceByMask(string source, string destination, string mask, IProgress<MaintenanceProgress> progress, CancellationToken ct)
        {
            int filesReplaced = await PerformMaintenance(source, destination,
                src => Directory.GetFiles(src, mask, SearchOption.TopDirectoryOnly),
                (src, dst, origFile, bads, exceptions, summaries) =>
                {
                    var originalFile = new FileInfo(origFile);
                    var destFile = new FileInfo(origFile.Replace(src, dst));
                    var needReplace = true;
                    if (!destFile.Exists)
                    {
                        bads.Add(destFile.FullName);
                        needReplace = false;
                    }
                    else if (destFile.IsReadOnly)
                    {
                        destFile.IsReadOnly = false;
                    }
                    originalFile.CopyTo(destFile.FullName, true);
                    return destFile.Exists && needReplace;
                },
                progress, ct);

            return filesReplaced;
        }

        public async Task<int> PerformMaintenance(
            string source, string destination,
            Func<string, string[]> filesFinder,
            Func<string, string, string, IList<string>, IList<string>, IList<string>, bool> fileProcessor,
            IProgress<MaintenanceProgress> progress, CancellationToken ct)
        {

            int filesProcessed = await Task.Run<int>(
                () =>
                {
                    var exceptions = new List<string>();
                    var bads = new List<string>();
                    var summaries = new List<string>();
                    string[] originalFiles;

                    try
                    {
                        originalFiles = filesFinder(source);
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e.ToString());
                        return 0;
                    }
                    int totalFiles = originalFiles.Count();
                    int justWalked = 0;
                    int reallyProcessed = 0;
                    Array.ForEach(originalFiles, (originalFileLocation) =>
                    {
                        int retryCount = 0;
                        bool tryAgain = false;

                        do
                        {
                            ct.ThrowIfCancellationRequested();
                            try
                            {
                                if (fileProcessor(source, destination, originalFileLocation, bads, exceptions, summaries))
                                {
                                    reallyProcessed++;
                                }
                            }
                            catch (Exception e)
                            {
                                tryAgain = true;
                                retryCount++;
                                if (retryCount > 12)
                                {
                                    tryAgain = false;
                                    var errStr = destination != null
                                        ? originalFileLocation.Replace(source, destination)
                                        : originalFileLocation;
                                    exceptions.Add(e.Message + Environment.NewLine + errStr);
                                }
                                else
                                {
                                    Thread.Sleep(250);
                                }
                            }
                        } while (tryAgain);

                        justWalked++;
                        if (progress != null)
                        {
                            progress.Report(new MaintenanceProgress()
                            {
                                Progress = justWalked * 100 / totalFiles,
                                Errors = exceptions,
                                Bads = bads,
                                Summaries = summaries,
                                QueueLength = totalFiles,
                                QueueComplete = justWalked
                            });
                        }
                    });
                    return reallyProcessed;
                }, ct);
            return filesProcessed;
        }

        public async Task<int> Maintenance10(string source, IProgress<MaintenanceProgress> progress, CancellationToken ct)
        {
            /*
             10.	Педаль 10 – удаляем все файлы, начинающиеся с DDTEMP, из корневого каталога SITEDATA (маска “DDTEMP*.*”);
             */

            int filesDeleted = await PerformMaintenance(source, null,
                (s) => Directory.GetFiles(source, "DDTEMP*.*", SearchOption.TopDirectoryOnly),
                (src, dest, origFile, bads, exceptions, summaries) =>
                {
                    var originalFile = new FileInfo(origFile);
                    if (originalFile.IsReadOnly)
                    {
                        originalFile.IsReadOnly = false;
                    }
                    File.Delete(origFile);
                    bads.Add(origFile);
                    return true;
                },
                progress, ct);

            return filesDeleted;
        }

    }
}
