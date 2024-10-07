using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;

namespace Game
{
    public class PerfCounter : Script
    {
        public bool FpsOnly = true;

        private Label _label;
        private string _gpuModel;
#if !BUILD_RELEASE
        private List<ProfilingTools.MainStats> _stats = new List<ProfilingTools.MainStats>(30);
#endif

        /// <inheritdoc />
        public override void OnEnable()
        {
            _label = Actor.As<UIControl>().Get<Label>();
            _gpuModel = GPUDevice.Instance.Adapter.Description;
#if !BUILD_RELEASE
            ProfilerGPU.Enabled = _label.Visible;
#endif
        }

        /// <inheritdoc />
        public override void OnDisable()
        {
            _label.Visible = true;
            _label = null;
#if !BUILD_RELEASE
            _stats.Clear();
#endif
        }

        /// <inheritdoc />
        public override void OnUpdate()
        {
            if (Input.GetKeyUp(KeyboardKeys.F2) || Input.GetGamepadButtonUp(InputGamepadIndex.All, GamepadButton.Y))
            {
                _label.Visible = !_label.Visible;
#if !BUILD_RELEASE
                _stats.Clear();
                ProfilerGPU.Enabled = _label.Visible;
#endif
            }

            if (!_label.Visible)
                return;
            _label.Text = string.Format("FPS: {0}", Engine.FramesPerSecond, _gpuModel);
            if (FpsOnly)
                return;

#if !BUILD_RELEASE
            // Average stats for smoother display
            if (_stats.Count == _stats.Capacity)
                _stats.RemoveAt(0);
            _stats.Add(ProfilingTools.Stats);
            var stats = _stats[0];
            for (int i = 1; i < _stats.Count; i++)
            {
                var e = _stats[i];
                stats.DrawGPUTimeMs += e.DrawGPUTimeMs;
                stats.DrawCPUTimeMs += e.DrawCPUTimeMs;
                stats.UpdateTimeMs += e.UpdateTimeMs;
                stats.PhysicsTimeMs += e.PhysicsTimeMs;
            }

            stats.DrawGPUTimeMs /= (float)_stats.Count;
            stats.DrawCPUTimeMs /= (float)_stats.Count;
            stats.UpdateTimeMs /= (float)_stats.Count;
            stats.PhysicsTimeMs /= (float)_stats.Count;

            // Show more stats in development build
            var cpuTotal = stats.UpdateTimeMs + stats.DrawCPUTimeMs + stats.PhysicsTimeMs;
            _label.Text += string.Format("FPS: {4}\nDraw GPU: {0} ms\nDraw CPU: {1} ms\nUpdate: {2} ms\nCPU Total: {3} ms", Mathf.Max((int)stats.DrawGPUTimeMs, 0), Mathf.Max((int)stats.DrawCPUTimeMs, 0), Mathf.Max((int)stats.UpdateTimeMs, 0), Mathf.Max((int)cpuTotal, 0), Engine.FramesPerSecond);
            _label.Height = 140;
#endif
        }
    }
}
