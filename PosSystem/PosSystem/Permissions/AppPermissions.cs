using System.Reflection;

namespace PosSystem.Permissions
{
    public static class AppPermissions
    {
        // MODULE: Tenants
        public static class Tenants
        {
            public const string View = "Permissions.Tenants.View";
            public const string Create = "Permissions.Tenants.Create";
            public const string Edit = "Permissions.Tenants.Edit";
            public const string Delete = "Permissions.Tenants.Delete";
            public const string ManagePlans = "Permissions.Tenants.ManagePlans";
        }

        // MODULE: Dashboard
        public static class Dashboard
        {
            public const string View = "Permissions.Dashboard.View";
            public const string ViewSalesStats = "Permissions.Dashboard.ViewSalesStats";
        }

        // MODULE: Products (Example)
        public static class Products
        {
            public const string View = "Permissions.Products.View";
            public const string Create = "Permissions.Products.Create";
            public const string Edit = "Permissions.Products.Edit";
            public const string Delete = "Permissions.Products.Delete";
        }

        // --- AUTO-DISCOVERY HELPER ---
        public static List<string> GetAllPermissions()
        {
            var permissions = new List<string>();
            var nestedClasses = typeof(AppPermissions).GetNestedTypes();

            foreach (var module in nestedClasses)
            {
                var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                foreach (var field in fields)
                {
                    var propertyValue = field.GetValue(null);
                    if (propertyValue is string permission)
                        permissions.Add(permission);
                }
            }
            return permissions;
        }
    }
}