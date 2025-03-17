/*using eft_dma_radar.Tarkov.GameWorld;
using eft_dma_shared.Common.DMA.ScatterAPI;
using eft_dma_shared.Common.Features;
using eft_dma_shared.Common.Misc;
using eft_dma_shared.Common.Misc.Commercial;
using eft_dma_shared.Common.Unity;
using System;

namespace eft_dma_radar.Tarkov.Features.MemoryWrites
{
    public sealed class Zoom : MemWriteFeature<Zoom>
    {
        private float _savedFOV;
        private bool _isApplied = false;

        public override bool Enabled
        {
            get => MemWrites.Config.Zoom;
            set => MemWrites.Config.Zoom = value;
        }

        protected override TimeSpan Delay => TimeSpan.FromMilliseconds(500); // Adjust delay as needed

        public override void TryApply(ScatterWriteHandle writes)
        {
            try
            {
                if (Memory.Game is LocalGameWorld game)
                {
                    ulong fpsCamera = game.CameraManager?.FPSCamera ?? 0x0;
                    fpsCamera.ThrowIfInvalidVirtualAddress();

                    if (Enabled)
                    {
                        if (!_isApplied)
                        {
                            // Save original FOV and apply configured FOV
                            _savedFOV = Memory.ReadValue<float>(fpsCamera + UnityOffsets.Camera.FOV);
                            int newFOV = MemWrites.Config.ZoomFOV; // Ensure this config setting exists
                            writes.AddValueEntry(fpsCamera + UnityOffsets.Camera.FOV, (float)newFOV);
                            _isApplied = true;
                            LoneLogging.WriteLine($"InstantZoom applied: FOV set to {newFOV}");
                        }
                    }
                    else
                    {
                        if (_isApplied)
                        {
                            // Restore original FOV
                            writes.AddValueEntry(fpsCamera + UnityOffsets.Camera.FOV, _savedFOV);
                            _isApplied = false;
                            LoneLogging.WriteLine($"InstantZoom restored: FOV reset to {_savedFOV}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoneLogging.WriteLine($"ERROR in InstantZoom: {ex}");
            }
        }
    }
}*/