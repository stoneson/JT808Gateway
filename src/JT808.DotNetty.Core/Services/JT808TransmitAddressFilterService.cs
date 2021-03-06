﻿using JT808.DotNetty.Abstractions.Dtos;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using JT808.DotNetty.Core.Configurations;

namespace JT808.DotNetty.Core.Services
{
    /// <summary>
    /// JT808转发地址过滤服务
    /// 按照808的消息，有些请求必须要应答，但是转发可以不需要有应答可以节省部分资源包括：
    //  1.消息的序列化
    //  2.消息的下发
    //  都有一定的性能损耗，那么不需要判断写超时 IdleState.WriterIdle 
    //  就跟神兽貔貅一样。。。
    /// </summary>
    public class JT808TransmitAddressFilterService : IDisposable
    {
        private readonly IOptionsMonitor<JT808Configuration> jT808ConfigurationOptionsMonitor;

        private ConcurrentDictionary<string,byte> ForwardingRemoteAddresssDict;

        private IDisposable jT808ConfigurationOptionsMonitorDisposable;

        public JT808TransmitAddressFilterService(
            IOptionsMonitor<JT808Configuration> jT808ConfigurationOptionsMonitor)
        {
            this.jT808ConfigurationOptionsMonitor = jT808ConfigurationOptionsMonitor;
            ForwardingRemoteAddresssDict = new ConcurrentDictionary<string, byte>();
            InitForwardingRemoteAddress(jT808ConfigurationOptionsMonitor.CurrentValue.ForwardingRemoteIPAddress);
            //OnChange 源码多播委托
            jT808ConfigurationOptionsMonitorDisposable = this.jT808ConfigurationOptionsMonitor.OnChange(options =>
            {
                ChangeForwardingRemoteAddress(options.ForwardingRemoteIPAddress);
            });
        }

        private void InitForwardingRemoteAddress(List<string> jT808ClientConfigurations)
        {
            if (jT808ClientConfigurations != null && jT808ClientConfigurations.Count > 0)
            {
                foreach (var item in jT808ClientConfigurations)
                {
                    ForwardingRemoteAddresssDict.TryAdd(item, 0);
                }
            }
        }

        private void ChangeForwardingRemoteAddress(List<string> jT808ClientConfigurations)
        {
            if (jT808ClientConfigurations != null && jT808ClientConfigurations.Count > 0)
            {
                ForwardingRemoteAddresssDict.Clear();
                foreach (var item in jT808ClientConfigurations)
                {             
                    ForwardingRemoteAddresssDict.TryAdd(item, 0);
                }
            }
            else
            {
                ForwardingRemoteAddresssDict.Clear();
            }
        }

        public bool ContainsKey(EndPoint endPoint)
        {
            IPAddress ip = ((IPEndPoint)endPoint).Address;
            return ForwardingRemoteAddresssDict.ContainsKey(ip.ToString());
        }

        public JT808ResultDto<bool> Add(string host)
        {
            return new JT808ResultDto<bool>() { Code = JT808ResultCode.Ok, Data = ForwardingRemoteAddresssDict.TryAdd(host,0) };
        }

        public JT808ResultDto<bool> Remove(string host)
        {
            if(jT808ConfigurationOptionsMonitor.CurrentValue.ForwardingRemoteIPAddress!=null &&
                jT808ConfigurationOptionsMonitor.CurrentValue.ForwardingRemoteIPAddress.Any(w=>w== host))
            {
                return new JT808ResultDto<bool>() { Code = JT808ResultCode.Ok, Data = false,Message="不能删除服务器配置的地址" };
            }
            else
            {
                return new JT808ResultDto<bool>() { Code = JT808ResultCode.Ok, Data = ForwardingRemoteAddresssDict.TryRemove(host,out var temp) };
            }
        }

        public JT808ResultDto<List<string>> GetAll()
        {
            return new JT808ResultDto<List<string>>(){ Code = JT808ResultCode.Ok, Data = ForwardingRemoteAddresssDict.Select(s=>s.Key).ToList() };
        }

        public void Dispose()
        {
            jT808ConfigurationOptionsMonitorDisposable.Dispose();
        }
    }
}
