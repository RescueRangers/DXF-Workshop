using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace DXF_Light.Model
{
    public class PlxFile : ObservableObject
    {
        #region PlxContents

        private const string Top = 
@"#!/usr/lectra/bin/lanceplx
# File generated with DXF workshop by Radoslaw Radomski 
#
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
3:1:0:0:0:0:0
#
#
# PQTY:PNUM:PNGR:PNVT:PNIB:PPNP:PCHV
#
4:1:__GROUPS__:10:10:0:0
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

        private const string Middle =
@"#
#
# VOPT:VNUM:VENT:VPAR:VNTP:VNPC
#
13:1:0:0:0:0
13:2:0:0:0:0
13:3:0:0:0:0
13:4:0:0:0:0
13:5:0:0:0:0
13:6:0:0:0:0
#
#
# IBA:INUM:VNUM:INLV:INAM:IEXT:IART:ICAN:ISIG:IFLG:IFLP:ISSS:ISPD:IFI1
#
";

        private const string Bottom = @"#
EOF";

        #endregion

        public ObservableCollection<DxfFile> DxfFiles
        {
            get => _dxfFiles;
            set => Set(ref _dxfFiles, value);
        }

        private readonly StringBuilder _vart = new StringBuilder();
        private readonly StringBuilder _iba = new StringBuilder();
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

        public void AddDxfToFile(IEnumerable<DxfFile> dxfFiles)
        {
            DxfFiles.AddRange(dxfFiles);
        }

        public IEnumerable<string> CreateFile(string plxFileName, PlxOptions options)
        {
            var plxFile = new StringBuilder(Top);
            plxFile = plxFile.Replace("__NAME__", plxFileName);  //Replace with the actual name
            var maxGroup = DxfFiles.Select(d => d.Group).Max();
            var safeLength = options.Length * 10;
            var safeWidth = options.Width * 10;
            plxFile = plxFile.Replace("__GROUPS__", maxGroup.ToString());  //Replace with the number of different groups
            plxFile = plxFile.Replace("__LENGTH__", safeLength.ToString("####")); //Replace length of material with actual value
            plxFile = plxFile.Replace("__WIDTH__", safeWidth.ToString("####")); // replace width of material with actual value
            var i = 0;
            var j = 0;
            foreach (var dxfFile in DxfFiles)
            {
                var vartLine = $"12:{i+1}:1:{dxfFile.Qty}:{dxfFile.Group}:0:0:{dxfFile.Name}:dxf";
                var ibaLine = $"14:{j+1}:1:{j}:{dxfFile.Name}::{dxfFile.Name}:::7:0:0:0:0";
                _vart.AppendLine(vartLine);
                _iba.AppendLine(ibaLine);
                
                if (dxfFile.Qty > 1)
                {
                    for (var k = 1; k < dxfFile.Qty; k++)
                    {
                        j++;
                        var ibaLineMultiple = $"14:{j+1}:1:{j}:{dxfFile.Name}::{dxfFile.Name}:::7:0:0:0:0";
                        _iba.AppendLine(ibaLineMultiple);
                    }
                }
                
                i++;
                j++;
            }

            plxFile.AppendLine(_vart.ToString());
            plxFile.AppendLine(Middle);
            plxFile.AppendLine(_iba.ToString());
            plxFile.AppendLine(Bottom);

            
            
            return plxFile.ToString().Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
        }

        #endregion

    }
}
