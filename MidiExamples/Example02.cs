﻿// Copyright (c) 2009, Tom Lokovic
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

using System;
using Midi;
using System.Threading;

namespace MidiExamples
{
    class Example02 : ExampleBase
    {
        public Example02()
            : base("Example02.cs", "Simple MIDI output example.")
        { }

        void PlayRunUpKeyboard(OutputDevice outputDevice, Predicate<int> predicate, int millisecondsBetween)
        {
            int previousNote = -1;
            for (int note = 21; note < 107; ++note)
            {
                if (predicate(note))
                {
                    if (previousNote != -1)
                    {
                        outputDevice.SendNoteOff(Channel.Channel1, previousNote, 80);
                    }
                    outputDevice.SendNoteOn(Channel.Channel1, note, 80);
                    Thread.Sleep(millisecondsBetween);
                    previousNote = note;
                }
            }
            if (previousNote != -1)
            {
                outputDevice.SendNoteOff(Channel.Channel1, previousNote, 80);
            }
        }

        public override void Run()
        {
            // Utility function prompts user to choose an output device (or if there is only one, returns that one).
            OutputDevice outputDevice = ExampleUtil.ChooseOutputDeviceFromConsole();
            if (outputDevice == null)
            {
                Console.WriteLine("No output devices, so can't run this example.");
                ExampleUtil.PressAnyKeyToContinue();
                return;
            }
            outputDevice.Open();

            Console.WriteLine("Playing an arpeggiated C chord and then bending it down.");
            // Play C, E, G in half second intervals.
            outputDevice.SendNoteOn(Channel.Channel1, 60, 80);
            Thread.Sleep(500);
            outputDevice.SendNoteOn(Channel.Channel1, 64, 80);
            Thread.Sleep(500);
            outputDevice.SendNoteOn(Channel.Channel1, 67, 80);
            Thread.Sleep(500);

            // Now apply the sustain pedal.
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 127);

            // Now release the C chord notes, but they should keep ringing because of the sustain pedal.
            outputDevice.SendNoteOff(Channel.Channel1, 60, 80);
            outputDevice.SendNoteOff(Channel.Channel1, 64, 80);
            outputDevice.SendNoteOff(Channel.Channel1, 67, 80);

            // Now bend the pitches down.
            for (int i = 0; i < 17; ++i)
            {
                outputDevice.SendPitchBend(Channel.Channel1, 8192 - i * 450);
                Thread.Sleep(200);
            }

            // Now release the sustain pedal, which should silence the notes.
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);

            Console.WriteLine("Playing sustained chord runs up the keyboard...");
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 127);
            PlayRunUpKeyboard(outputDevice, note => note % 12 == 0 || note % 12 == 4 || note % 12 == 7, 100);
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 127);
            PlayRunUpKeyboard(outputDevice, note => note % 12 == 5 || note % 12 == 9 || note % 12 == 12, 100);
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 127);
            PlayRunUpKeyboard(outputDevice, note => note % 12 == 7 || note % 12 == 11 || note % 12 == 14, 100);
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 127);
            PlayRunUpKeyboard(outputDevice, note => note % 12 == 0 || note % 12 == 4 || note % 12 == 7, 100);
            Thread.Sleep(2000);
            outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);

            // Close the output device.
            outputDevice.Close();

            // All done.
            Console.WriteLine();
            ExampleUtil.PressAnyKeyToContinue();
        }
    }
}
