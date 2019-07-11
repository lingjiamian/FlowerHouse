using FlowerHouse.Models.Entity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlowerHouse.Models
{
    public class SampleData
    {
        public static async Task InitializeBlogDatabaseAsync(IServiceProvider serviceProvider, bool createUser = true)
        {
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var scropeServiceProvider = serviceScope.ServiceProvider;
                var db = scropeServiceProvider.GetService<FlowerHouseContext>();
                if (await db.Database.EnsureCreatedAsync())
                {
                    await InsertTestData(scropeServiceProvider);
                    if (createUser)
                    {
                        await CreateAdminUserAsync(scropeServiceProvider);
                    }
                }
            }
        }

        private static async Task CreateAdminUserAsync(IServiceProvider serviceProvider)
        {
            var env = serviceProvider.GetService<IHostingEnvironment>();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            var userManager = serviceProvider.GetService<UserManager<HouseUser>>();

            var adminUser = await userManager.FindByNameAsync(configuration["DefaultAdminUsername"]);

            //创建管理员用户
            if (adminUser == null)
            {
                adminUser = new HouseUser { UserName = configuration["DefaultAdminUsername"], Id = "15015015011", PhoneNumber = "15015015011", PhoneNumberConfirmed = true };
                await userManager.CreateAsync(adminUser, configuration["DefaultAdminPassword"]);
                await userManager.AddClaimAsync(adminUser, new Claim("ManageHouse", "Allowed"));
            }

            //创建十个普通用户
            string lingjiamian = configuration["DefaultUsername"];
            string userPwd = configuration["DefaultUserPassword"];
            string[] userid = new string[10];
            string id = "1832016651";
            Random ran = new Random();
            for (int i = 0; i < 10; i++)
            {
                userid[i] = id + i;
            }
            for (int i = 0; i < 10; i++)
            {
                HouseUser user = new HouseUser() { Id = userid[i], UserName = lingjiamian + i, PhoneNumber = userid[i], PhoneNumberConfirmed = true };
                await userManager.CreateAsync(user, userPwd);
            }
        }

        private async static Task InsertTestData(IServiceProvider scropeServiceProvider)
        {
            List<FlowerTag> flowerTags = GetFlowerTag(FlowerDict, TagDict);
            //插入的顺序要注意，主键表一定要在外键表之前插入数据库，比如Assortment表一定要在Tag表之前插入数据库
            List<Stock> stocks = GetStocks(FlowerDict);
            await AddOrUpdateAsync(scropeServiceProvider, AssortmentDict.Select(a => a.Value));
            await AddOrUpdateAsync(scropeServiceProvider, FlowerDict.Select(f => f.Value));
            await AddOrUpdateAsync(scropeServiceProvider, TagDict.Select(t => t.Value));
            await AddOrUpdateAsync(scropeServiceProvider, flowerTags);
            await AddOrUpdateAsync(scropeServiceProvider, stocks);

        }

        private static List<Stock> GetStocks(Dictionary<string, Flower> flowerDict)
        {
            List<Stock> stocks = new List<Stock>();
            Random random = new Random();
            foreach (var dict in flowerDict)
            {
                int sellPrice = random.Next(100, 500);
                stocks.Add(new Stock
                {
                    Flower = dict.Value,
                    StockCount = random.Next(100, 300),
                    SellPrice = sellPrice,
                    PurchasePrice = random.Next(sellPrice - 100, sellPrice)
                });
            }
            return stocks;
        }

        private static async Task AddOrUpdateAsync<TEntity>(
            IServiceProvider serviceProvider, IEnumerable<TEntity> entities)
            where TEntity : class
        {
            // Query in a separate context so that we can attach existing entities as modified
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetService<FlowerHouseContext>();
                foreach (var entity in entities)
                {
                    db.Entry(entity).State = EntityState.Added;
                }
                //不能用这个
                //await db.AddRangeAsync(entities);
                await db.SaveChangesAsync();
            }
        }



        private static Dictionary<string, Tag> tagDict;
        public static Dictionary<string, Tag> TagDict
        {
            get
            {
                if (tagDict == null)
                {
                    List<Tag> tagList = new List<Tag>
                    {
                        new Tag{ TagName = "爱情鲜花",Assortment=AssortmentDict["鲜花用途"]},
                        new Tag{ TagName = "生日鲜花",Assortment=AssortmentDict["鲜花用途"]},
                        new Tag{ TagName = "友情鲜花",Assortment=AssortmentDict["鲜花用途"]},
                        new Tag{ TagName = "祝贺鲜花",Assortment=AssortmentDict["鲜花用途"]},
                        new Tag{ TagName = "婚庆鲜花",Assortment=AssortmentDict["鲜花用途"]},
                        new Tag{ TagName = "商务鲜花",Assortment=AssortmentDict["鲜花用途"]},
                        new Tag{ TagName = "道歉鲜花",Assortment=AssortmentDict["鲜花用途"]},
                        new Tag{ TagName = "问候长辈",Assortment=AssortmentDict["鲜花用途"]},
                        new Tag{ TagName = "探病慰问",Assortment=AssortmentDict["鲜花用途"]},
                        new Tag{ TagName = "康乃馨",Assortment=AssortmentDict["鲜花花材"]},
                        new Tag{ TagName = "郁金香",Assortment=AssortmentDict["鲜花花材"]},
                        new Tag{ TagName = "马蹄铁",Assortment=AssortmentDict["鲜花花材"]},
                        new Tag{ TagName = "向日葵",Assortment=AssortmentDict["鲜花花材"]},
                        new Tag{ TagName = "玫瑰",Assortment=AssortmentDict["鲜花花材"]},
                        new Tag{ TagName = "百合",Assortment=AssortmentDict["鲜花花材"]},
                        new Tag{ TagName = "扶郎",Assortment=AssortmentDict["鲜花花材"] },
                        new Tag{ TagName = "开业花篮",Assortment=AssortmentDict["鲜花类别"]},
                        new Tag{ TagName = "桌面花篮",Assortment=AssortmentDict["鲜花类别"]},
                        new Tag{ TagName = "精品鲜花",Assortment=AssortmentDict["鲜花类别"]},
                        new Tag{ TagName = "瓶花",Assortment=AssortmentDict["鲜花类别"]},
                        new Tag{ TagName = "果篮",Assortment=AssortmentDict["鲜花类别"]},
                        new Tag{ TagName = "经典花盒",Assortment=AssortmentDict["永生之花"]},
                        new Tag{ TagName = "巨型玫瑰",Assortment=AssortmentDict["永生之花"]},
                        new Tag{ TagName = "薰衣草",Assortment=AssortmentDict["永生之花"]},
                        new Tag{ TagName = "音乐睡枕",Assortment=AssortmentDict["其他礼品"]},
                        new Tag{ TagName = "保鲜花",Assortment=AssortmentDict["其他礼品"]},
                        new Tag{ TagName = "水晶内雕",Assortment=AssortmentDict["其他礼品"]},
                        new Tag{ TagName = "绿色植物",Assortment=AssortmentDict["其他礼品"]},
                        new Tag{ TagName = "音乐盒",Assortment=AssortmentDict["其他礼品"]},
                    };

                    foreach (var tag in tagList)
                    {
                        tag.TagImg = "/images/icons/" + tag.TagName + ".jpg";
                    }

                    tagDict = new Dictionary<string, Tag>();
                    foreach (var tag in tagList)
                    {
                        tagDict.Add(tag.TagName, tag);
                    }
                }
                return tagDict;
            }
        }


        private static List<FlowerTag> GetFlowerTag(Dictionary<string, Flower> flowerDict, Dictionary<string, Tag> tagDict)
        {
            List<FlowerTag> flowerTags = new List<FlowerTag>();
            for (int i = 0; i <= 10; i++)
            {
                flowerTags.Add(new FlowerTag { Flower = flowerDict["初心不负" + i], Tag = tagDict["爱情鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["初心不负" + i], Tag = tagDict["友情鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["初心不负" + i], Tag = tagDict["玫瑰"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["初心不负" + i], Tag = tagDict["保鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["一缕清香" + i], Tag = tagDict["保鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["一缕清香" + i], Tag = tagDict["问候长辈"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["一缕清香" + i], Tag = tagDict["探病慰问"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["一缕清香" + i], Tag = tagDict["经典花盒"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["馨情无限" + i], Tag = tagDict["经典花盒"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["馨情无限" + i], Tag = tagDict["精品鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["馨情无限" + i], Tag = tagDict["马蹄铁"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["馨情无限" + i], Tag = tagDict["友情鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["馨情无限" + i], Tag = tagDict["生日鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["留住好时光" + i], Tag = tagDict["生日鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["留住好时光" + i], Tag = tagDict["生日鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["留住好时光" + i], Tag = tagDict["生日鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["留住好时光" + i], Tag = tagDict["生日鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["留住好时光" + i], Tag = tagDict["生日鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["留住好时光" + i], Tag = tagDict["生日鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["留住好时光" + i], Tag = tagDict["生日鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["一心一意" + i], Tag = tagDict["友情鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["一心一意" + i], Tag = tagDict["爱情鲜花"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["一心一意" + i], Tag = tagDict["百合"] });
                flowerTags.Add(new FlowerTag { Flower = flowerDict["一心一意" + i], Tag = tagDict["桌面花篮"] });
            }
            return flowerTags;
        }

        //用字典比较好
        private static Dictionary<string, Flower> flowerDict;
        public static Dictionary<string, Flower> FlowerDict
        {
            get
            {
                if (flowerDict == null)
                {
                    List<Flower> flowers = new List<Flower>();

                    for (int i = 0; i <= 10; i++)
                    {
                        flowers.Add(new Flower { FlowerName = "一心一意" + i, FlowerBuy = 513, DistributionRange = "全国配送", FlowerEmblem = "喜欢你", FlowerImg = "/images/flowerImg/yixinyiyi.jpg", FlowerMaterial = "玫瑰" });
                        flowers.Add(new Flower { FlowerName = "初心不负" + i, FlowerBuy = 123, DistributionRange = "全国配送", FlowerEmblem = "忠贞不渝", FlowerImg = "/images/flowerImg/chuxinbufu.jpg", FlowerMaterial = "玫瑰" });
                        flowers.Add(new Flower { FlowerName = "一缕清香" + i, FlowerBuy = 761, DistributionRange = "全国配送", FlowerEmblem = "喜欢你", FlowerImg = "/images/flowerImg/yilvqingxiang.jpg", FlowerMaterial = "菊花" });
                        flowers.Add(new Flower { FlowerName = "馨情无限" + i, FlowerBuy = 421, DistributionRange = "全国配送", FlowerEmblem = "亲近与关怀", FlowerImg = "/images/flowerImg/xinqingwuxian.jpg", FlowerMaterial = "康乃馨" });
                        flowers.Add(new Flower { FlowerName = "留住好时光" + i, FlowerBuy = 726, DistributionRange = "全国配送", FlowerEmblem = "无限精彩", FlowerImg = "/images/flowerImg/liuzhuhaoshiguang.jpg", FlowerMaterial = "百合" });
                    }
                    flowerDict = new Dictionary<string, Flower>();
                    foreach (var flower in flowers)
                    {
                        flowerDict.Add(flower.FlowerName, flower);
                    }
                }
                return flowerDict;
            }
        }

        private static Dictionary<string, Assortment> assortmentDict;
        public static Dictionary<string, Assortment> AssortmentDict
        {

            get
            {
                if (assortmentDict == null)
                {
                    List<Assortment> assortments = new List<Assortment>
                    {
                        new Assortment{AssortmentName="鲜花用途"},
                        new Assortment{AssortmentName="鲜花花材"},
                        new Assortment{AssortmentName="鲜花类别"},
                        new Assortment{AssortmentName="永生之花"},
                        new Assortment{AssortmentName="其他礼品"}
                    };
                    assortmentDict = new Dictionary<string, Assortment>();
                    foreach (var ass in assortments)
                    {
                        assortmentDict.Add(ass.AssortmentName, ass);
                    }
                }
                return assortmentDict;
            }
        }
    }
}
