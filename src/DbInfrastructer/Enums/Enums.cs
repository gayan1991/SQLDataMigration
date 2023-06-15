using System.ComponentModel;

namespace Db.Infrastructure.Enums
{
    public enum DBTypeEnum : byte
    {
        [Description("Config DB")]
        ConfigDb = 0,
        [Description("Report DB")]
        ReportDb = 1,
        [Description("Ref DB")]
        RefDb = 2
    }

    public enum DbEnd : byte
    {
        Source = 0,
        Target = 1
    }
}
