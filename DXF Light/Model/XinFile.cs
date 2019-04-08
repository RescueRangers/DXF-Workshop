using System;
using System.Collections.Generic;
using System.Text;

namespace DXF_Light.Model
{
    public static class XinFileBuilder
    {
        private const string FileStructure = @"#
# DC_TechTex_V4R1c3
#
# {0:dd-MM-yyyy},{1:HH:mm:ss}
#
VAR_IN:VNAME_IN:VACN_IN:VCN_IN:GENRE_IN
1:Product1:::

ART_IN:ANAME_IN:VNAME_IN:PNAME_IN:ASPQ_IN:ADPQ_IN:AFT_IN:ASF_IN:ARV_IN:ASG_IN
2:{2}:Product1:{2}:1:0:1:0:0:

PRT_IN:PNAME_IN:PBFN_IN:PACN_IN
3:{2}:{2}:

EOF";

        public static IEnumerable<string> CreateXin(string dxfName)
        {
            var xinBuilder = new StringBuilder();

            xinBuilder.AppendLine(string.Format(FileStructure, DateTime.UtcNow, DateTime.Now, dxfName));


            return xinBuilder.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None); ;
        }
    }
}
