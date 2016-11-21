﻿using MockOrleans.Grains;
using MockOrleans.Reminders;
using MockOrleans.Streams;
using Orleans;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockOrleans
{
    
    public class MockFixture
    {
        public readonly FixtureScheduler Scheduler;
        public readonly RequestRegistry Requests;
        public readonly ExceptionSink Exceptions;
        
        public readonly TypeMap Types;
        public readonly MockSerializer Serializer;
        public readonly GrainRegistry Grains;
        public readonly BootstrapperRegistry Bootstrappers;
        public readonly ReminderRegistry Reminders;
        public readonly StorageRegistry Stores;
        public readonly StreamRegistry Streams;

        public readonly ServiceRegistry Services;
        public readonly IGrainFactory GrainFactory;


        TaskFactory _taskFac;

        public MockFixture(IServiceProvider services = null) 
        {
            Serializer = new MockSerializer(this);
            Exceptions = new ExceptionSink();
            Scheduler = new FixtureScheduler(Exceptions);
            Requests = new RequestRegistry(Scheduler);
            Services = new ServiceRegistry(services);          
            Types = new TypeMap(this);
            GrainFactory = new MockGrainFactory(this);
            Stores = new StorageRegistry(Serializer);
            Grains = new GrainRegistry(this);
            Streams = new StreamRegistry(Grains, Types);
            Reminders = new ReminderRegistry(this);
            Bootstrappers = new BootstrapperRegistry(this);

            _taskFac = new TaskFactory(Scheduler);
        }

                

        public GrainProxy GetGrainProxy(ResolvedGrainKey key) {
            return GrainProxy.Proxify(this, key);
        }



        public Task Start()
            => _taskFac.StartNew(() => Bootstrappers.Init()).Unwrap();

        public Task Stop()
            => _taskFac.StartNew(() => Bootstrappers.Close()).Unwrap();

    }
}