using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace DXF_Light.Model
{
    public class NoContourDxf : ObservableObject
    {
        #region DxfHead

        private const string DxfHead = @"  0
SECTION
  2
HEADER
  9
$ACADVER
  1
AC1006
  9
$DWGCODEPAGE
  3
ansi_1250
  9
$INSBASE
 10
0.0
 20
0.0
 30
0.0
  9
$EXTMIN
 10
1.000000000000000E+20
 20
1.000000000000000E+20
  9
$EXTMAX
 10
-1.000000000000000E+20
 20
-1.000000000000000E+20
  9
$LIMMIN
 10
0.0
 20
0.0
  9
$LIMMAX
 10
12.0
 20
9.0
  9
$ORTHOMODE
 70
     0
  9
$REGENMODE
 70
     1
  9
$FILLMODE
 70
     1
  9
$QTEXTMODE
 70
     0
  9
$MIRRTEXT
 70
     1
  9
$DRAGMODE
 70
     2
  9
$LTSCALE
 40
1.0
  9
$OSMODE
 70
    37
  9
$ATTMODE
 70
     1
  9
$TEXTSIZE
 40
0.2
  9
$TRACEWID
 40
0.05
  9
$TEXTSTYLE
  7
STANDARD
  9
$CLAYER
  8
0
  9
$CELTYPE
  6
BYLAYER
  9
$CECOLOR
 62
   256
  9
$DIMSCALE
 40
1.0
  9
$DIMASZ
 40
0.18
  9
$DIMEXO
 40
0.0625
  9
$DIMDLI
 40
0.38
  9
$DIMRND
 40
0.0
  9
$DIMDLE
 40
0.0
  9
$DIMEXE
 40
0.18
  9
$DIMTP
 40
0.0
  9
$DIMTM
 40
0.0
  9
$DIMTXT
 40
0.18
  9
$DIMCEN
 40
0.09
  9
$DIMTSZ
 40
0.0
  9
$DIMTOL
 70
     0
  9
$DIMLIM
 70
     0
  9
$DIMTIH
 70
     1
  9
$DIMTOH
 70
     1
  9
$DIMSE1
 70
     0
  9
$DIMSE2
 70
     0
  9
$DIMTAD
 70
     0
  9
$DIMZIN
 70
     0
  9
$DIMBLK
  1

  9
$DIMASO
 70
     1
  9
$DIMSHO
 70
     1
  9
$DIMPOST
  1

  9
$DIMAPOST
  1

  9
$DIMALT
 70
     0
  9
$DIMALTD
 70
     2
  9
$DIMALTF
 40
25.4
  9
$DIMLFAC
 40
1.0
  9
$DIMTOFL
 70
     0
  9
$DIMTVP
 40
0.0
  9
$DIMTIX
 70
     0
  9
$DIMSOXD
 70
     0
  9
$DIMSAH
 70
     0
  9
$DIMBLK1
  1

  9
$DIMBLK2
  1

  9
$LUNITS
 70
     2
  9
$LUPREC
 70
     4
  9
$SKETCHINC
 40
0.1
  9
$FILLETRAD
 40
0.5
  9
$AUNITS
 70
     0
  9
$AUPREC
 70
     0
  9
$MENU
  1
.
  9
$ELEVATION
 40
0.0
  9
$THICKNESS
 40
0.0
  9
$LIMCHECK
 70
     0
  9
$CHAMFERA
 40
0.5
  9
$CHAMFERB
 40
0.5
  9
$SKPOLY
 70
     0
  9
$TDCREATE
 40
2458507.441175035
  9
$TDUPDATE
 40
2458507.441175046
  9
$TDINDWG
 40
0.0000000116
  9
$TDUSRTIMER
 40
0.0000000116
  9
$USRTIMER
 70
     1
  9
$ANGBASE
 50
0.0
  9
$ANGDIR
 70
     0
  9
$PDMODE
 70
     0
  9
$PDSIZE
 40
0.0
  9
$PLINEWID
 40
0.0
  9
$COORDS
 70
     1
  9
$SPLFRAME
 70
     0
  9
$SPLINETYPE
 70
     6
  9
$SPLINESEGS
 70
     8
  9
$ATTDIA
 70
     0
  9
$ATTREQ
 70
     1
  9
$HANDLING
 70
     1
  9
$HANDSEED
  5
5E
  9
$SURFTAB1
 70
     6
  9
$SURFTAB2
 70
     6
  9
$SURFTYPE
 70
     6
  9
$SURFU
 70
     6
  9
$SURFV
 70
     6
  9
$UCSNAME
  2

  9
$UCSORG
 10
0.0
 20
0.0
 30
0.0
  9
$UCSXDIR
 10
1.0
 20
0.0
 30
0.0
  9
$UCSYDIR
 10
0.0
 20
1.0
 30
0.0
  9
$USERI1
 70
     0
  9
$USERI2
 70
     0
  9
$USERI3
 70
     0
  9
$USERI4
 70
     0
  9
$USERI5
 70
     0
  9
$USERR1
 40
0.0
  9
$USERR2
 40
0.0
  9
$USERR3
 40
0.0
  9
$USERR4
 40
0.0
  9
$USERR5
 40
0.0
  9
$WORLDVIEW
 70
     1
  0
ENDSEC
  0
SECTION
  2
TABLES
  0
TABLE
  2
VPORT
 70
     1
  0
VPORT
  2
*ACTIVE
 70
     0
 10
0.0
 20
0.0
 11
1.0
 21
1.0
 12
10.42990654205607
 22
4.5
 13
0.0
 23
0.0
 14
0.5
 24
0.5
 15
0.5
 25
0.5
 16
0.0
 26
0.0
 36
1.0
 17
0.0
 27
0.0
 37
0.0
 40
9.0
 41
1.972972972850329
 42
50.0
 43
0.0
 44
0.0
 50
0.0
 51
0.0
 71
     0
 72
   100
 73
     1
 74
     3
 75
     0
 76
     0
 77
     0
 78
     0
  0
ENDTAB
  0
TABLE
  2
LTYPE
 70
     3
  0
LTYPE
  2
CONTINUOUS
 70
     0
  3
Solid line
 72
    65
 73
     0
 40
0.0
  0
LTYPE
  2
________________
 70
     0
  3

 72
    65
 73
     0
 40
0.0
  0
LTYPE
  2
________________0
 70
     0
  3

 72
    65
 73
     4
 40
7.2
 49
2.7
 49
-1.8
 49
0.8999999999999999
 49
-1.8
  0
ENDTAB
  0
TABLE
  2
LAYER
 70
     4
  0
LAYER
  2
0
 70
     0
 62
     7
  6
CONTINUOUS
  0
LAYER
  2
1
 70
     0
 62
     7
  6
CONTINUOUS
  0
LAYER
  2
7
 70
     0
 62
     7
  6
CONTINUOUS
  0
LAYER
  2
11
 70
     0
 62
     7
  6
CONTINUOUS
  0
ENDTAB
  0
TABLE
  2
STYLE
 70
     1
  0
STYLE
  2
STANDARD
 70
     0
 40
0.0
 41
1.0
 50
0.0
 71
     0
 42
0.2
  3
txt
  4

  0
ENDTAB
  0
TABLE
  2
VIEW
 70
     0
  0
ENDTAB
  0
TABLE
  2
UCS
 70
     0
  0
ENDTAB
  0
ENDSEC
  0
SECTION
  2
BLOCKS
  0
ENDSEC
  0
SECTION
  2
ENTITIES
  0
POLYLINE
  5
3E
  8
1
  6
________________
 62
     9
 66
     1
 10
0.0
 20
0.0
 30
0.0
 70
     1
  0
VERTEX
  5
59
  8
1
  6
________________
";

        #endregion

        #region Lines

        private const string Vertex = @" 62
     9
 10
{0}
 20
{1}
 30
0.0
  0
VERTEX
  5
{2:X}
  8
1
  6
________________";

        private const string VertesEnd = @" 62
     9
 10
{0}
 20
{1}
 30
0.0
  0
SEQEND
  5
{2:X}
  8
1
  6
________________
 62
     9
  0
LINE
  5
40
  8
11
  6
________________";

        private const string Line = @" 62
   212
 10
{0}
 20
0.0
 30
0.0
 11
{1}
 21
{2}
 31
0.0
  0
LINE
  5
{3:X}
  8
11
  6
________________";

        private const string EndOfFile = @" 62
    52
 10
628.9557017700643
 20
507.0213959482022
 30
0.0
 11
1128.955701770064
 21
507.0213959482022
 31
0.0
  0
ENDSEC
  0
EOF";

        #endregion

        public bool IsValid => ValidateDxf();

        public ObservableCollection<InternalCut> InternalCuts
        {
            get => _internalCuts;
            set => Set(ref _internalCuts, value);
        }

        public decimal Height
        {
            get => _height;
            set => Set(ref _height, value);
        }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private decimal _width;
        private ObservableCollection<InternalCut> _internalCuts = new ObservableCollection<InternalCut>();
        private decimal _height;
        private string _name;

        private bool ValidateDxf()
        {
            if (Height != 0 && !string.IsNullOrWhiteSpace(Name) && InternalCuts.Count > 1)
            {
                return AreAllCutsPositive();
            }
            return false;
        }

        private bool AreAllCutsPositive()
        {
            foreach (var internalCut in InternalCuts)
            {
                if (internalCut.Cut < 1)
                {
                    return false;
                }
            }

            return true;
        }

        public string CreateDxf()
        {
            var dxfBuilder = new StringBuilder(DxfHead);

            var cutLength = InternalCuts.Sum(s => s.Cut);

            
            dxfBuilder.AppendLine(string.Format(Vertex, -1, Height + 1, 90));
            dxfBuilder.AppendLine(string.Format(Vertex, -1, -1, 91));
            dxfBuilder.AppendLine(string.Format(Vertex, cutLength + 1, -1, 92));
            dxfBuilder.AppendLine(string.Format(VertesEnd, cutLength + 1, Height + 1, 93));

            dxfBuilder.AppendLine(string.Format(Line, 0, 0, Height, 65));

            decimal previousCut = 0;

            for (var i = 0; i < InternalCuts.Count; i++)
            {
                var cut = InternalCuts[i].Cut + previousCut;

                dxfBuilder.AppendLine(string.Format(Line, cut, cut, Height, i + 66));

                previousCut = cut;
            }

            dxfBuilder.Length = dxfBuilder.Length - 2;
            dxfBuilder.AppendLine("0");
            dxfBuilder.AppendLine(EndOfFile);

            return dxfBuilder.ToString();
        }

    }
}
