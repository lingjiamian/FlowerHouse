using FlowerHouse.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlowerHouse.BgService
{
    /// <summary>
    /// 关闭超时订单的服务，每30分钟执行一次
    /// </summary>
    public class OrderTimeOutService : BackgroundService
    {
        private FlowerHouseContext flowerHouseContext;
        private readonly IServiceProvider serviceProvider;

        public OrderTimeOutService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var serviceScope = serviceProvider.CreateScope();
                var scropeServiceProvider = serviceScope.ServiceProvider;
                flowerHouseContext = scropeServiceProvider.GetService<FlowerHouseContext>();
                await flowerHouseContext.Orders.ForEachAsync(o =>
                 {
                     if (o.OrderStatus == OrderStatus.待支付)
                     {
                         if (DateTime.Now.CompareTo(o.TimeOutTime) >= 0)
                         {
                             o.OrderStatus = OrderStatus.已关闭;
                         }
                     }
                 });
                flowerHouseContext.SaveChanges();
                serviceScope.Dispose();
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }
}
