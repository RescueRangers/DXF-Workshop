using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DXF_Light.Model
{
    public class PlxFile : ObservableObject
    {
        #region PlxContents

        private const string _top =
@"#!/usr/lectra/bin/lanceplx
# File generated with DXF workshop by Radoslaw Radomski
#
__CONTENTS__
#
#
# UNT:UPOS:USRF:UANG:CVER:UVER:UKEY
#
1::::102:102:\Bl0YBcBL@VkJOYGbRW1mJtC7S`pLOn10
#
#
# PLC:PNUM:TNUM:PNAM:PCOD:PTMC:PDAC:PTMT:PDAT:PCOM
#
2:1:1:__NAME__:A:1529910876:25062018:1529911047:25062018
#
#
# PGEO:PNUM:PESP:PDEP:PROT:PEFD:PVEC
#
3:1:0:0:0:0:1
#
#
# PQTY:PNUM:PNGR:PNVT:PNIB:PPNP:PCHV
#
4:1:__GROUPS__:5:5:0:0
#
#
# PSTA:PNUM:PNCP:PLGR:PLRD:PEFR:PPER:PLID:PLIC:PPSV:PAIR:PANG:PCRA:PNPT
#
5:1:0:0:0:0:0:0:0:0:0:0:0:0
#
#
# TIS:TNUM:KNUM:TCON:TNAM:TCOD:TTYP:TCOU
#
6:1:1:0:::1:0
#
#
# LEZ:TNUM:LLEZ:LLNG:LLIS:LLPX:LLNX:LLPY:LLNY
#
7:1:__WIDTH__:__LENGTH__:0:0:0:0:0
#
#
# EXP:PNUM:XMOD:XTMP:XESS:XPAR:XSHA
#
8:1:0:0:0::0
#
#
# ACCESS:PNUM:CMDL:CVET:CIBA:CCTR:CALP
#
9:1
#
#
# OPT:PNUM:OQUA:OCRI:OCRV
#
10:1:0:0:0
#
#
# KARO:KNUM:KTIS:KDEY:KORY:KPTX:KPTY
#
11:1:0:0:0:-1:-1
#
#
# VART:VNUM:PNUM:VQTY:VGRP:VGEN:VSNS:VGFN:VEXT:VCAN:VCOD:VCNF:VSTA:VCOM
#
";
        private const string _fileContents = @"
UNT:UPOS:USRF:UANG:CVER:UVER:UKEY
PLC:PNUM:TNUM:PNAM:PCOD:PTMC:PDAC:PTMT:PDAT:PCOM
PGEO:PNUM:PESP:PDEP:PROT:PEFD:PVEC
PQTY:PNUM:PNGR:PNVT:PNIB:PPNP:PCHV
PSTA:PNUM:PNCP:PLGR:PLRD:PEFR:PPER:PLID:PLIC:PPSV:PAIR:PANG:PCRA:PNPT
TIS:TNUM:KNUM:TCON:TNAM:TCOD:TTYP:TCOU
LEZ:TNUM:LLEZ:LLNG:LLIS:LLPX:LLNX:LLPY:LLNY
EXP:PNUM:XMOD:XTMP:XESS:XPAR:XSHA
ACCESS:PNUM:CMDL:CVET:CIBA:CCTR:CALP
OPT:PNUM:OQUA:OCRI:OCRV
KARO:KNUM:KTIS:KDEY:KORY:KPTX:KPTY
VART:VNUM:PNUM:VQTY:VGRP:VGEN:VSNS:VGFN:VEXT:VCAN:VCOD:VCNF:VSTA:VCOM
VOPT:VNUM:VENT:VPAR:VNTP:VNPC
IBA:INUM:VNUM:INLV:INAM:IEXT:IART:ICAN:ISIG:IFLG:IFLP:ISSS:ISPD:IFI1
";
        private const string _igeoContent = "IGEO:INUM:IXOR:IXOD:IYOR:IYOD:IXDI:IYDI:ISRF:IPER:IGRS:IGRF:IIRS:IIRF:IFRS:IFRF:IVRS:IVRF";
        private const string _imopContent = "IMOP:INUM:ISVX:ISVY:ISDX:ISDY:IPAX:IPAY:ICUT:ICRA:ISNS:CNAM:IFSD";

        private const string _vopt =
@"#
#
# VOPT:VNUM:VENT:VPAR:VNTP:VNPC
#
";

        private const string _iba = @"#
#
# IBA:INUM:VNUM:INLV:INAM:IEXT:IART:ICAN:ISIG:IFLG:IFLP:ISSS:ISPD:IFI1
#
";

        private const string _igeoLine = @"
#
# IGEO:INUM:IXOR:IXOD:IYOR:IYOD:IXDI:IYDI:ISRF:IPER:IGRS:IGRF:IIRS:IIRF:IFRS:IFRF:IVRS:IVRF
#
";
        private const string _imopLine = @"
#
# IMOP:INUM:ISVX:ISVY:ISDX:ISDY:IPAX:IPAY:ICUT:ICRA:ISNS:CNAM:IFSD
#
";

        private const string _bottom = @"#
EOF";

        #endregion PlxContents

        public ObservableCollection<DxfFile> DxfFiles
        {
            get => _dxfFiles;
            set => _dxfFiles = value;
        }

        private readonly StringBuilder _vartSB = new StringBuilder();
        private readonly StringBuilder _ibaSB = new StringBuilder(_iba);
        private readonly StringBuilder _igeoSB = new StringBuilder();
        private readonly StringBuilder _imopSB = new StringBuilder();
        private readonly StringBuilder _voptSB = new StringBuilder(_vopt);
        private readonly StringBuilder _contentSB = new StringBuilder(_fileContents);
        private ObservableCollection<DxfFile> _dxfFiles;

        public PlxFile(IList<DxfFile> dxfFiles)
        {
            if (DxfFiles == null)
            {
                DxfFiles = new ObservableCollection<DxfFile>(dxfFiles);
            }
            else
            {
                DxfFiles.AddRange(dxfFiles);
            }
        }

        #region Methods

        /// <summary>
        /// Add Dxf files to be processed in to plx file. Throws <see cref="ArgumentNullException"/> if <paramref name="dxfFiles"/> is null or empty.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="dxfFiles"/> can not be null or empty.</exception>
        /// <param name="dxfFiles"></param>
        public void AddDxfToFile(IEnumerable<DxfFile> dxfFiles)
        {
            if (dxfFiles?.Any() != true) throw new ArgumentNullException(nameof(dxfFiles));

            DxfFiles.AddRange(dxfFiles);
        }

        public IEnumerable<string> CreateFile(string plxFileName, PlxOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var plxFile = new StringBuilder(_top);
            plxFile = plxFile.Replace("__NAME__", plxFileName);  //Replace with the actual name
            var maxGroup = DxfFiles.Select(d => d.Group).Max();
            var safeLength = options.Length * 10;
            var safeWidth = options.Width * 10;
            plxFile = plxFile.Replace("__GROUPS__", maxGroup.ToString(CultureInfo.InvariantCulture))
                .Replace("__LENGTH__", safeLength.ToString("####", CultureInfo.InvariantCulture))
                .Replace("__WIDTH__", safeWidth.ToString("####", CultureInfo.InvariantCulture));  //Replace with the number of different groups
                                                                                                  //Replace length of material with actual value
                                                                                                  //replace width of material with actual value
            FilllNestingFile(options);

            plxFile.AppendLine(_vartSB.ToString());
            plxFile.AppendLine(_voptSB.ToString());
            plxFile.AppendLine(_ibaSB.ToString());
            if (options.PatchesNesting)
            {
                _contentSB.AppendLine(_igeoContent);
                _contentSB.AppendLine(_imopContent);
                plxFile.AppendLine(_igeoLine);
                plxFile.AppendLine(_igeoSB.ToString());
                plxFile.AppendLine(_imopLine);
                plxFile.AppendLine(_imopSB.ToString());
            }
            plxFile.Replace("__CONTENTS__", _contentSB.ToString());
            plxFile.AppendLine(_bottom);

            return plxFile.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        }

        private void FilllNestingFile(PlxOptions options)
        {
            var ibaSB = new StringBuilder(_iba);
            var i = 0;
            var j = 0;
            var currentLength = 0;
            IEnumerable<DxfFile> sortedDxfs = null;
            if (options.RealizeAscending)
            {
                sortedDxfs = DxfFiles.OrderBy(x => x.Name);
            }
            else
            {
                sortedDxfs = DxfFiles.OrderByDescending(x => x.Name);
            }
            foreach (var dxfFile in sortedDxfs)
            {
                var ibaLine = "";
                var vartLine = $"12:{i + 1}:1:{dxfFile.Qty}:{dxfFile.Group}:0:0:{dxfFile.Name}:dxf";
                _voptSB.AppendLine($"13:{i + 1}:1:0:1:1");
                if (options.PatchesNesting)
                {
                    ibaLine = $"14:{j + 1}:{j + 1}:{j}:{dxfFile.Name}::{dxfFile.Name}:::0:0:0:0:0";
                }
                else
                {
                    ibaLine = $"14:{j + 1}:1:{j}:{dxfFile.Name}::{dxfFile.Name}:::7:0:0:0:0";
                }
                _vartSB.AppendLine(vartLine);
                _ibaSB.AppendLine(ibaLine);

                if (options.PatchesNesting && double.TryParse(dxfFile.Length, out var length) && double.TryParse(dxfFile.Width, out var width))
                {
                    var igeoLine = $"15:{j + 1}:{currentLength}:{currentLength}:50:50:{(j == 0 ? length * 10 : currentLength+(j * 10) + 40)}:{(int)(width * 10)}:{j}:0:0:0:0:0:0:0:0:0";
                    var imopLine = $"16:{j + 1}:{length / 2*10}:-50:500:{options.Width * 10}:-1:-1:0:0:0::0";
                    _igeoSB.AppendLine(igeoLine);
                    _imopSB.AppendLine(imopLine);
                    currentLength += (int)(length * 10);
                }

                if (dxfFile.Qty > 1)
                {
                    for (var k = 1; k < dxfFile.Qty; k++)
                    {
                        j++;
                        var ibaLineMultiple = $"14:{j + 1}:1:{j}:{dxfFile.Name}::{dxfFile.Name}:::7:0:0:0:0";
                        _ibaSB.AppendLine(ibaLineMultiple);
                    }
                }

                i++;
                j++;
            }
        }

        public IEnumerable<string> CreateOnePlxPerDxf(PlxOptions options, DxfFile dxf)
        {
            if (dxf == null)
            {
                throw new ArgumentNullException(nameof(dxf));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var plxFile = new StringBuilder(_top);
            var safeLength = options.Length * 10;
            string plxFileName;

            if (options.SameWidth)
            {
                plxFileName = $"{dxf.Name}({options.Width})";

                plxFile = plxFile.Replace("__GROUPS__", "1")                                              //Replace with the number of different groups
                .Replace("__LENGTH__", safeLength.ToString("####", CultureInfo.InvariantCulture))    //Replace length of material with actual value
                .Replace("__WIDTH__", $"{options.Width * 10}");                                 // replace width of material with actual value
            }
            else
            {
                plxFileName = options.SameWidth ? $"{dxf.Name}({options.Width})" : $"{dxf.Name}({double.Parse(dxf.Width, CultureInfo.InvariantCulture) + 100})";

                plxFile = plxFile.Replace("__GROUPS__", "1")                                              //Replace with the number of different groups
                .Replace("__LENGTH__", safeLength.ToString("####", CultureInfo.InvariantCulture))        //Replace length of material with actual value
                .Replace("__WIDTH__", $"{(double.Parse(dxf.Width, CultureInfo.InvariantCulture) + 10) * 10}");                                      // replace width of material with actual value
            }

            plxFile = plxFile.Replace("__NAME__", plxFileName);  //Replace with the actual name

            var vartLine = $"12:1:1:{dxf.Qty}:{dxf.Group}:0:0:{dxf.Name}:dxf";
            var vopt = new StringBuilder(_voptSB.ToString());
            vopt.AppendLine("13:1:1:0:1:1");
            var iba = new StringBuilder(_ibaSB.ToString());
            iba.AppendLine($"14:1:1:1:{dxf.Name}::{dxf.Name}:::7:0:0:0:0");

            plxFile.AppendLine(vartLine);
            plxFile.AppendLine(vopt.ToString());
            plxFile.AppendLine(iba.ToString());
            plxFile.AppendLine(_bottom);
            plxFile.Replace("__CONTENTS__", _contentSB.ToString());

            return plxFile.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        }

        #endregion Methods
    }
}