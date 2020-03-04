using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using DXF_Light.Model;
using DXF_Light.Properties;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace DXF_Light.Servicess
{
    public class IOService : IIOService
    {
        private const string ResourceFile = "DXF_Light.template.dxf";
        private readonly int[] _lineNumbers = { 733, 735, 749, 751, 765, 767, 781, 783 };

        public string OpenFileDialog(string defaultPath, string fileFilter)
        {
            var fod = new OpenFileDialog
            {
                Filter = fileFilter,
                InitialDirectory = defaultPath
            };

            var results = fod.ShowDialog();

            if (results != true) return string.Empty;

            var fileInfo = new FileInfo(fod.FileName);
            Settings.Default.InitialFolder = fileInfo.DirectoryName;
            Settings.Default.Save();
            return fod.FileName;
        }

        public void Message(string messageText, string title)
        {
            MessageBox.Show(messageText, title, MessageBoxButton.OK);
        }

        public void GetFolder(Action<string, Exception> callback)
        {
            var fb = new FolderBrowserDialog();

            if (!string.IsNullOrWhiteSpace(Settings.Default.InitialFolder))
            {
                fb.SelectedPath = Settings.Default.InitialFolder;
            }

            if (fb.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.InitialFolder = fb.SelectedPath;
                Settings.Default.Save();
                callback(fb.SelectedPath, null);
            }
            else
            {
                callback(fb.SelectedPath, new Exception("Nie wybrano folderu do zapisu."));
            }
        }

        public void GetDxfFiles(Action<List<DxfFile>, string, Exception> callback)
        {
            var fod = new OpenFileDialog
            {
                Filter = "Pliki dxf (.dxf)|*.dxf",
                Multiselect = true
            };

            var dxfFiles = new List<DxfFile>();

            var results = fod.ShowDialog();

            if (results == true)
            {
                foreach (var fileName in fod.FileNames)
                {
                    var dxfFileInfo = new FileInfo(fileName);

                    var firstLengthBool = double.TryParse(File.ReadLines(fileName).Skip(733).Take(1).First(),
                        NumberStyles.Float, CultureInfo.InvariantCulture, out var firstLength);
                    var secondLengthBool = double.TryParse(File.ReadLines(fileName).Skip(765).Take(1).First(),
                        NumberStyles.Float, CultureInfo.InvariantCulture, out var secondLength);

                    var firstWidthBool = double.TryParse(File.ReadLines(fileName).Skip(735).Take(1).First(),
                        NumberStyles.Float, CultureInfo.InvariantCulture, out var firstWidth);
                    var secondWidthBool = double.TryParse(File.ReadLines(fileName).Skip(767).Take(1).First(),
                        NumberStyles.Float, CultureInfo.InvariantCulture, out var secondWidth);

                    if (!(firstWidthBool && secondWidthBool && firstLengthBool && secondLengthBool))
                    {
                        dxfFiles.Add(new DxfFile
                        {
                            Name = dxfFileInfo.Name.Remove(dxfFileInfo.Name.Length - 4)
                        });
                        continue;
                    }

                    var dxfFile = new DxfFile
                    {
                        Name = dxfFileInfo.Name.Remove(dxfFileInfo.Name.Length - 4),
                        Length = Math.Round(AbsoluteValue(firstLength, secondLength)).ToString(CultureInfo.InvariantCulture),
                        Width = Math.Round(AbsoluteValue(firstWidth, secondWidth)).ToString(CultureInfo.InvariantCulture)
                    };

                    dxfFiles.Add(dxfFile);
                }

                var fileInfo = new FileInfo(fod.FileName);

                callback(dxfFiles, fileInfo.DirectoryName, null);
            }
            else
            {
                callback(null, null, new Exception("Nie wybrano żadnego pliku."));
            }
        }

        public void GetDxfFilesFromPaths(Action<List<DxfFile>, string, Exception> callback, List<string> paths)
        {
            var dxfFiles = new List<DxfFile>();
            var directoryName = "";

            foreach (var fileName in paths)
            {
                var dxfFileInfo = new FileInfo(fileName);

                if (!dxfFileInfo.Exists)
                {
                    callback(null, null, new Exception(Properties.Resources.FileNotExist));
                    return;
                }

                var firstLengthBool = double.TryParse(File.ReadLines(fileName).Skip(733).Take(1).First(),
                    NumberStyles.Float, CultureInfo.InvariantCulture, out var firstLength);
                var secondLengthBool = double.TryParse(File.ReadLines(fileName).Skip(765).Take(1).First(),
                    NumberStyles.Float, CultureInfo.InvariantCulture, out var secondLength);

                var firstWidthBool = double.TryParse(File.ReadLines(fileName).Skip(735).Take(1).First(),
                    NumberStyles.Float, CultureInfo.InvariantCulture, out var firstWidth);
                var secondWidthBool = double.TryParse(File.ReadLines(fileName).Skip(767).Take(1).First(),
                    NumberStyles.Float, CultureInfo.InvariantCulture, out var secondWidth);

                if (!(firstWidthBool && secondWidthBool && firstLengthBool && secondLengthBool))
                {
                    dxfFiles.Add(new DxfFile
                    {
                        Name = dxfFileInfo.Name.Remove(dxfFileInfo.Name.Length - 4)
                    });
                    directoryName = dxfFileInfo.DirectoryName;
                    continue;
                }

                var dxfFile = new DxfFile
                {
                    Name = dxfFileInfo.Name.Remove(dxfFileInfo.Name.Length - 4),
                    Length = Math.Round(AbsoluteValue(firstLength, secondLength)).ToString(),
                    Width = Math.Round(AbsoluteValue(firstWidth, secondWidth)).ToString()
                };

                dxfFiles.Add(dxfFile);
                directoryName = dxfFileInfo.DirectoryName;
            }

            callback(dxfFiles, directoryName, null);
        }

        public void CreatePlx(Action<Exception> callback, PlxFile plxFile, string path)
        {
            throw new NotImplementedException();
        }

        public void CreatePlyDxf(Action<Exception> callback, List<PlyFile> plyFiles, string path)
        {
            var assembly = Assembly.GetExecutingAssembly();

            try
            {
                foreach (var file in plyFiles)
                {
                    var linesToWrite = new List<string>();

                    var destination = file.Material != null ? $@"{path}\{file.Material}\" : $@"{path}\";
                    Directory.CreateDirectory(destination);

                    var upperLength = (file.L3 + file.L4) + (file.L1 - file.L3);

                    linesToWrite.Add(upperLength.ToString("F1", CultureInfo.InvariantCulture));
                    linesToWrite.Add(file.W.ToString("F1", CultureInfo.InvariantCulture));
                    linesToWrite.Add((file.L1 - file.L3).ToString("F1", CultureInfo.InvariantCulture));
                    linesToWrite.Add(file.W.ToString("F1", CultureInfo.InvariantCulture));
                    linesToWrite.Add("0.0");
                    linesToWrite.Add("0.0");
                    linesToWrite.Add((file.L1 + file.L2).ToString("F1", CultureInfo.InvariantCulture));
                    linesToWrite.Add("0.0");

                    // Read from the target file and write to a new file.
                    var lineNumber = 0;

                    // ReSharper disable once AssignNullToNotNullAttribute
                    using (var reader = new StreamReader(assembly.GetManifestResourceStream(ResourceFile)))
                    {
                        using (var writer = new StreamWriter(destination + file.Name + ".dxf"))
                        {
                            var i = 0;
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (i < 8 && lineNumber == _lineNumbers[i])
                                {
                                    writer.WriteLine(linesToWrite[i]);
                                    i++;
                                }
                                else
                                {
                                    writer.WriteLine(line);
                                }
                                lineNumber++;
                            }
                        }
                    }
                }
                callback(null);
            }
            catch (Exception e)
            {
                callback(e);
            }
        }

        public void CreateNCDxf(Action<Exception> callback, NoContourDxf nCDxf, string path)
        {
            try
            {
                File.WriteAllText(path + @"\" + nCDxf.Name + ".dxf", nCDxf.CreateDxf());
            }
            catch (Exception e)
            {
                callback(e);
            }
        }

        public void CreateDxfFiles(Action<Exception> callback, List<DxfFile> dxfFiles, string path)
        {
            var assembly = Assembly.GetExecutingAssembly();

            try
            {
                foreach (var file in dxfFiles)
                {
                    var linesToWrite = new List<string>();

                    var destination = file.Material != null ? $@"{path}\{file.Material}\" : $@"{path}\";
                    Directory.CreateDirectory(destination);

                    var hToWrite = (double.Parse(file.Length) / 2).ToString("F1", CultureInfo.InvariantCulture);
                    var wToWrite = (double.Parse(file.Width) / 2).ToString("F1", CultureInfo.InvariantCulture);
                    linesToWrite.Add("-" + hToWrite);
                    linesToWrite.Add("-" + wToWrite);
                    linesToWrite.Add(hToWrite);
                    linesToWrite.Add("-" + wToWrite);
                    linesToWrite.Add(hToWrite);
                    linesToWrite.Add(wToWrite);
                    linesToWrite.Add("-" + hToWrite);
                    linesToWrite.Add(wToWrite);

                    // Read from the target file and write to a new file.
                    var lineNumber = 0;

                    // ReSharper disable once AssignNullToNotNullAttribute
                    using (var reader = new StreamReader(assembly.GetManifestResourceStream(ResourceFile)))
                    {
                        using (var writer = new StreamWriter(destination + file.Name + ".dxf"))
                        {
                            var i = 0;
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (i < 8 && lineNumber == _lineNumbers[i])
                                {
                                    writer.WriteLine(linesToWrite[i]);
                                    i++;
                                }
                                else
                                {
                                    writer.WriteLine(line);
                                }
                                lineNumber++;
                            }
                        }
                    }
                }
                callback(null);
            }
            catch (Exception e)
            {
                callback(e);
            }
        }

        private static double AbsoluteValue(double firstNumber, double secondNumber)
        {
            return firstNumber >= secondNumber ? Math.Abs(firstNumber - secondNumber) : Math.Abs(secondNumber - firstNumber);
        }
    }
}