using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace REghZyAccountManagerV6.Core.IdleEvents {
    public class IdleEventService : IDisposable {
        public delegate void BeginActionEvent();

        public event BeginActionEvent OnIdle;

        private DateTime lastInput;

        private volatile bool canFireEvent;

        private volatile bool stopTask;

        public TimeSpan MinimumTimeSinceInput { get; set; }

        public TimeSpan TaskTickInterval { get; set; }

        public bool CanFireNextTick {
            get => this.canFireEvent;
            set => this.canFireEvent = value;
        }

        private readonly Task task;

        public IdleEventService() {
            this.MinimumTimeSinceInput = TimeSpan.FromMilliseconds(200);
            this.TaskTickInterval = TimeSpan.FromMilliseconds(100);
            this.task = Task.Run(async () => {
                await this.Main();
            });
        }

        private async Task Main() {
            while (true) {
                if (this.stopTask) {
                    return;
                }

                if ((DateTime.Now - this.lastInput) > this.MinimumTimeSinceInput) {
                    if (this.canFireEvent) {
                        this.canFireEvent = false;
                        try {
                            await IoC.Dispatcher.InvokeAsync(this.FireEvent);
                        }
                        catch (ThreadAbortException) {
                            return;
                        }
                        catch (Exception e) {
                            Debugger.Break();
                            Debug.WriteLine("Exception while processing idle event: " + e.Message);
                        }
                    }
                }

                await Task.Delay(this.TaskTickInterval);
            }
        }

        public void FireEvent() {
            this.OnIdle?.Invoke();
        }

        public void OnInput() {
            if (!this.canFireEvent) {
                this.canFireEvent = true;
            }

            this.lastInput = DateTime.Now;
        }

        public void ForceAction() {
            this.canFireEvent = false;
            this.lastInput = DateTime.Now;
            this.FireEvent();
        }

        public void Dispose() {
            this.stopTask = true;
            try {
                this.task.Wait(1000);
            }
            catch { }
        }
    }
}