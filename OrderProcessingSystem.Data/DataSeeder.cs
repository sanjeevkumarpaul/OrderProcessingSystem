using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

    const int target = 20; // suppliers/customers target
    const int desiredOrders = 200; // aim to have ~200 orders in the DB for realistic reports

        // Ensure at least `target` suppliers
        var supplierCount = await db.Suppliers.CountAsync();
        var sampleSuppliers = new[] {
            "Acme Corp","Globex Inc","Umbrella LLC","Stark Industries","Wayne Enterprises",
            "Wonka Industries","Tyrell Corp","Initech","Hooli","Vehement Capital",
            "Roxxon","Cyberdyne","Oceanic Airlines","Soylent Corp","Pied Piper",
            "Vandelay Industries","Massive Dynamic","Virtucon","Gringotts","Prestige Worldwide"
        };

        if (supplierCount < target)
        {
            var suppliers = new List<Supplier>();
            for (int i = 0; i < target; i++)
            {
                var idx = i % sampleSuppliers.Length;
                suppliers.Add(new Supplier { Name = sampleSuppliers[idx], Country = (i % 3 == 0) ? "USA" : (i % 3 == 1 ? "Germany" : "China") });
            }
            db.Suppliers.AddRange(suppliers);
            await db.SaveChangesAsync();
        }
        else
        {
            var existing = await db.Suppliers.OrderBy(s => s.SupplierId).ToListAsync();
            for (int i = 0; i < Math.Min(existing.Count, target); i++)
            {
                existing[i].Name = sampleSuppliers[i % sampleSuppliers.Length];
                existing[i].Country = (i % 3 == 0) ? "USA" : (i % 3 == 1 ? "Germany" : "China");
            }
            await db.SaveChangesAsync();
        }

        // Ensure at least `target` customers
        var customerCount = await db.Customers.CountAsync();
        var sampleCustomers = new[] {
            "Olivia Smith","Liam Johnson","Emma Williams","Noah Brown","Ava Jones",
            "Lucas Garcia","Mia Miller","Mason Davis","Sophia Rodriguez","Ethan Martinez",
            "Isabella Hernandez","Logan Lopez","Amelia Gonzalez","James Wilson","Harper Anderson",
            "Benjamin Thomas","Evelyn Taylor","Elijah Moore","Abigail Jackson","Alexander Martin"
        };

        if (customerCount < target)
        {
            var customers = new List<Customer>();
            for (int i = 0; i < target; i++)
            {
                var idx = i % sampleCustomers.Length;
                customers.Add(new Customer { Name = sampleCustomers[idx] });
            }
            db.Customers.AddRange(customers);
            await db.SaveChangesAsync();
        }
        else
        {
            var existing = await db.Customers.OrderBy(c => c.CustomerId).ToListAsync();
            for (int i = 0; i < Math.Min(existing.Count, target); i++)
            {
                existing[i].Name = sampleCustomers[i % sampleCustomers.Length];
            }
            await db.SaveChangesAsync();
        }

        // Ensure we have a healthy number of orders for realistic reporting
        var orderCount = await db.Orders.CountAsync();
        var rnd = new Random();
        var sampleProductPrices = new[] { 9.99m, 12.50m, 19.99m, 24.99m, 4.75m, 99.99m, 5.50m };
        var statuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };

        // determine a safe starting order number (above any existing OrderId and at least 1000)
        var maxExisting = await db.Orders.MaxAsync(o => (int?)o.OrderId) ?? 999;
        int nextOrderNumber = Math.Max(1000, maxExisting + 1);

        if (orderCount < desiredOrders)
        {
            var suppliers = await db.Suppliers.ToListAsync();
            var customers = await db.Customers.ToListAsync();
            var orders = new List<Order>();

            int toCreate = desiredOrders - orderCount;
            // create `toCreate` orders, distributing randomly across customers and suppliers
            for (int i = 0; i < toCreate; i++)
            {
                var cust = customers[rnd.Next(customers.Count)];
                // bias supplier choice slightly towards a supplier derived from the customer id for realism
                var pref = (cust.CustomerId - 1) % suppliers.Count;
                var sup = suppliers[(pref + rnd.Next(suppliers.Count)) % suppliers.Count];

                int itemsCount = rnd.Next(1, 6); // 1-5 distinct items
                decimal totalDec = 0m;
                for (int it = 0; it < itemsCount; it++)
                {
                    var price = sampleProductPrices[rnd.Next(sampleProductPrices.Length)];
                    var qty = rnd.Next(1, 6);
                    totalDec += price * qty;
                }
                // small random fee or discount
                totalDec += (decimal)((rnd.NextDouble() - 0.2) * 8.0);

                var total = Math.Round((double)Math.Max(1.0m, totalDec), 2);

                orders.Add(new Order
                {
                    OrderId = nextOrderNumber++,
                    CustomerId = cust.CustomerId,
                    SupplierId = sup.SupplierId,
                    Total = total,
                    Status = statuses[rnd.Next(statuses.Length)]
                });
            }

            db.Orders.AddRange(orders);
            await db.SaveChangesAsync();
        }
        else
        {
            // If we already have >= desiredOrders, refresh a subset (up to desiredOrders) to ensure varied totals/status
            var existing = await db.Orders.OrderBy(o => o.OrderId).ToListAsync();
            int refreshCount = Math.Min(existing.Count, desiredOrders);
            for (int i = 0; i < refreshCount; i++)
            {
                var order = existing[i];
                int itemsCount = rnd.Next(1, 6);
                decimal totalDec = 0m;
                for (int it = 0; it < itemsCount; it++)
                {
                    var price = sampleProductPrices[rnd.Next(sampleProductPrices.Length)];
                    var qty = rnd.Next(1, 6);
                    totalDec += price * qty;
                }
                totalDec += (decimal)((rnd.NextDouble() - 0.2) * 8.0);
                order.Total = Math.Round((double)Math.Max(1.0m, totalDec), 2);
                order.Status = statuses[rnd.Next(statuses.Length)];

                // ensure order ids are in the readable range
                if (order.OrderId < 1000)
                {
                    order.OrderId = nextOrderNumber++;
                }
            }
            await db.SaveChangesAsync();
        }

        // Seed UserLog data with 20,000 records spanning last 2 years
        const int desiredUserLogs = 20000;
        var userLogCount = await db.UserLogs.CountAsync();
        
        if (userLogCount < desiredUserLogs)
        {
            var userLogs = new List<UserLog>();
            var events = new[] {
                "MANAGER", "ADMIN", "USER", "FAILED"
            };
            
            var eventFlags = new[] {
                "AUTH", "ORDER", "PRODUCT", "PAYMENT", "ACCOUNT", "FILE", "REPORT", "SYSTEM", "API", "SECURITY"
            };
            
            var companyDomains = new[] {
                "acmecorp.com", "globexltd.com", "stellartech.com", "pinnaclegroup.com", "nexusinnovations.com",
                "vanguardenterprises.com", "meridiantech.com", "apexsolutions.com", "titanindustries.com", "omegacorp.com",
                "deltatech.com", "infinitelogistics.com", "primeventures.com", "elitemanufacturing.com", "summitconsulting.com",
                "phoenixtech.com", "quantumcorp.com", "fusionenterprises.com", "catalystgroup.com", "dynamicsolutions.com"
            };
            
            var firstNames = new[] {
                "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda", "William", "Elizabeth",
                "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah", "Christopher", "Karen",
                "Charles", "Nancy", "Daniel", "Lisa", "Matthew", "Betty", "Anthony", "Helen", "Mark", "Sandra",
                "Donald", "Donna", "Steven", "Carol", "Paul", "Ruth", "Andrew", "Sharon", "Joshua", "Michelle",
                "Kenneth", "Laura", "Kevin", "Sarah", "Brian", "Kimberly", "George", "Deborah", "Edward", "Dorothy"
            };
            
            var lastNames = new[] {
                "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
                "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
                "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
                "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
                "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts"
            };
            
            var startDate = DateTime.Now.AddYears(-2); // 2 years ago
            var endDate = DateTime.Now;
            var totalDays = (endDate - startDate).TotalDays;
            
            int toCreate = desiredUserLogs - userLogCount;
            
            for (int i = 0; i < toCreate; i++)
            {
                // Generate random date within the last 2 years
                var randomDays = rnd.NextDouble() * totalDays;
                var eventDate = startDate.AddDays(randomDays);
                
                // Select random first name, last name, and company domain
                var firstName = firstNames[rnd.Next(firstNames.Length)];
                var lastName = lastNames[rnd.Next(lastNames.Length)];
                var domain = companyDomains[rnd.Next(companyDomains.Length)];
                var userEmail = $"{firstName.ToLower()}.{lastName.ToLower()}@{domain}";
                var userName = $"{firstName} {lastName}";
                
                userLogs.Add(new UserLog
                {
                    EventDate = eventDate,
                    Event = events[rnd.Next(events.Length)],
                    EventFlag = eventFlags[rnd.Next(eventFlags.Length)],
                    UserId = userEmail,
                    UserName = userName
                });
                
                // Add in batches of 1000 to avoid memory issues
                if (userLogs.Count >= 1000)
                {
                    db.UserLogs.AddRange(userLogs);
                    await db.SaveChangesAsync();
                    userLogs.Clear();
                }
            }
            
            // Add any remaining records
            if (userLogs.Any())
            {
                db.UserLogs.AddRange(userLogs);
                await db.SaveChangesAsync();
            }
        }
    }
}
