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

        // MODULE: Users
        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
        }

        // MODULE: Roles
        public static class Roles
        {
            public const string View = "Permissions.Roles.View";
            public const string Create = "Permissions.Roles.Create";
            public const string Edit = "Permissions.Roles.Edit";
            public const string Delete = "Permissions.Roles.Delete";
        }

        // MODULE: Products (The Items themselves)
        public static class Products
        {
            public const string View = "Permissions.Products.View";
            public const string Create = "Permissions.Products.Create";
            public const string Edit = "Permissions.Products.Edit";
            public const string Delete = "Permissions.Products.Delete";
        }

        // [NEW] MODULE: Categories (The Groups/Zones)
        public static class Categories
        {
            public const string View = "Permissions.Categories.View";
            public const string Create = "Permissions.Categories.Create";
            public const string Edit = "Permissions.Categories.Edit";
            public const string Delete = "Permissions.Categories.Delete";
        }

        // MODULE: Plans (Subscriptions)
        public static class Plans
        {
            public const string View = "Permissions.Plans.View";
            public const string Create = "Permissions.Plans.Create";
            public const string Edit = "Permissions.Plans.Edit";
            public const string Delete = "Permissions.Plans.Delete";
        }

        // MODULE: Reports
        public static class Reports
        {
            public const string View = "Permissions.Reports.View";
            public const string Sales = "Permissions.Reports.Sales";
            public const string Inventory = "Permissions.Reports.Inventory";
        }

        // [NEW] MODULE: Terminology (The Naming Brain)
        public static class Terminology
        {
            public const string View = "Permissions.Terminology.View";
            public const string Edit = "Permissions.Terminology.Edit";
        }

        // [NEW] MODULE: Business Categories (Super Admin: Zoo, Restaurant definitions)
        public static class BusinessCategories
        {
            public const string View = "Permissions.BusinessCategories.View";
            public const string Create = "Permissions.BusinessCategories.Create";
            public const string Edit = "Permissions.BusinessCategories.Edit";
            public const string Delete = "Permissions.BusinessCategories.Delete";
        }

        // [NEW] MODULE: Units (The Physical Branches)
        public static class Units
        {
            public const string View = "Permissions.Units.View";
            public const string Create = "Permissions.Units.Create";
            public const string Edit = "Permissions.Units.Edit";
            public const string Delete = "Permissions.Units.Delete";
        }

        // [NEW] MODULE: UnitTypes (Definitions like Kitchen, Gate)
        public static class UnitTypes
        {
            public const string View = "Permissions.UnitTypes.View";
            public const string Create = "Permissions.UnitTypes.Create";
            public const string Edit = "Permissions.UnitTypes.Edit";
            public const string Delete = "Permissions.UnitTypes.Delete";
        }
        // [NEW] MODULE: Point of Sale
        public static class Pos
        {
            // The policy we used in the Layout
            public const string Access = "Permissions.Pos.Access";
        }

        // [NEW] MODULE: Orders
        public static class Orders
        {
            public const string View = "Permissions.Orders.View";
            public const string Create = "Permissions.Orders.Create"; // Taking an order
            public const string Edit = "Permissions.Orders.Edit";     // Modifying (if allowed)
            public const string Delete = "Permissions.Orders.Delete"; // Voiding
        }
        public static class Inventory
        {
            public const string View = "Permissions.Inventory.View";
            public const string Edit = "Permissions.Inventory.Edit"; // Adjustment
        }
        public static class Settings
        {
            public const string General = "Permissions.Settings.General"; // Store Name, etc.
            public const string Receipts = "Permissions.Settings.Receipts"; // <--- FOR YOUR RECEIPT PAGE
            public const string Payments = "Permissions.Settings.Payments"; // Razorpay Keys (Sensitive!)
        }
        public static class Facilities
        {
            public const string View = "Permissions.Facilities.View";
            public const string GenerateQr = "Permissions.Facilities.GenerateQr";
            public const string Gatekeeper = "Permissions.Facilities.Gatekeeper";
        }
        public static class Members
        {
            public const string View = "Permissions.Members.View";
            public const string Register = "Permissions.Members.Register";
            public const string Edit = "Permissions.Members.Edit";
            public const string Delete = "Permissions.Members.Delete";
        }
        public static class Catalog
        {
            public const string MeasurementUnits = "Permissions.Catalog.MeasurementUnits";
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