﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace NStore.Persistence
{
    public class PollingClient
    {
        private CancellationTokenSource _source;
        private readonly IPersistence _store;
        private readonly ISubscription _subscription;
        public int Delay { get; set; }
        long _lastScan = 0;

        public long Position => _lastScan;

        public PollingClient(IPersistence store, ISubscription subscription)
        {
            _subscription = subscription;
            _store = store;
            Delay = 200;
        }

        public void Stop()
        {
            _source.Cancel();
        }

        public void Start()
        {
            _source = new CancellationTokenSource();
            var token = _source.Token;

            var wrapper = new LambdaSubscription((data) =>
            {
                // retry if out of sequence
                if (data.Position != _lastScan+1)
                    return Task.FromResult(false);

                _lastScan = data.Position;
                return _subscription.OnNext(data);
            });

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    await this._store.ReadAllAsync(
                        _lastScan + 1,
                        wrapper,
                        int.MaxValue,
                        token
                    );
                    await Task.Delay(Delay, token);
                }
            }, token);
        }
    }
}
