using d9.utl.compat;

namespace d9.ktv;
public class ActivityCategoryDef
{
    public GoogleUtils.EventColor? EventColor { get; set; }
    public required List<ActivityDef> ActivityDefs { get; set; }
}