﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JT808.Gateway.Services
{
    public class JT808ClientReportHostedService : IHostedService
    {
        private readonly JT808ClientReportService jT808ReportService;
        private CancellationTokenSource cts = new CancellationTokenSource();
        public JT808ClientReportHostedService(JT808ClientReportService jT808ReportService)
        {
            this.jT808ReportService = jT808ReportService;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    jT808ReportService.Create();
                    Thread.Sleep(1000);
                    //Task.Delay(TimeSpan.FromSeconds(1), cts.Token);
                }
            }, cts.Token);
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            cts.Cancel();
            return Task.CompletedTask;
        }
    }
}
