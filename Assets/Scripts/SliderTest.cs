using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class AudioDeviceManager : MonoBehaviour
{
    // Define Win32 API functions
    [DllImport("winmm.dll", SetLastError = true)]
    static extern uint waveOutGetNumDevs();

    [DllImport("winmm.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern uint waveOutGetDevCaps(uint hwo, ref WAVEOUTCAPS pwoc, uint cbwoc);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct WAVEOUTCAPS
    {
        public ushort wMid;
        public ushort wPid;
        public uint vDriverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szPname;
        public uint dwFormats;
        public ushort wChannels;
        public ushort wReserved1;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            EnumerateAudioDevices();
        }
    }

    // Method to enumerate audio devices
    public static void EnumerateAudioDevices()
    {
        uint deviceCount = waveOutGetNumDevs();
        for (uint i = 0; i < deviceCount; i++)
        {
            WAVEOUTCAPS caps = new WAVEOUTCAPS();
            if (waveOutGetDevCaps(i, ref caps, (uint)Marshal.SizeOf(caps)) == 0)
            {
                Debug.Log("Audio Device " + i + ": " + caps.szPname);
            }
        }
    }
}
