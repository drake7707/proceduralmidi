using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace ProceduralMidi.DAL
{
    public static class MidiWriter
    {
        private const Int16 MIDI_FORMAT = 0x00;
        private const Int16 NR_OF_TRACKS_USED = 0x01;
        private const int MILLISECONDS_PER_MINUTE = 60000;

        private const int BEATS_PER_MINUTE = 120;
        private const int TICKS_PER_BEAT = 240;

        private const byte MSG_NOTE_DOWN = 0x9;
        private const byte MSG_NOTE_UP = 0x8;
        private const byte MSG_PROGRAM_CHANGE = 0xC;

        private static ulong GetTicks(TimeSpan t)
        {
            const double BEAT_PER_MILLISECOND = ((double)BEATS_PER_MINUTE / (double)MILLISECONDS_PER_MINUTE);
            const double TICKS_PER_MILLISECOND = TICKS_PER_BEAT * BEAT_PER_MILLISECOND;

            return (ulong)(t.TotalMilliseconds * TICKS_PER_MILLISECOND);
        }



        public static void Write(string path, int instrumentIdx,  List<Note> notes)
        {
            using (Stream stream = File.Create(path))
            {
                BinaryWriter writer = new BinaryWriter(stream);
                WriteHeader(writer);

                WriteTrack(writer, instrumentIdx, notes);
            }

        }

        private static void WriteHeader(BinaryWriter writer)
        {
            // write chunk ID
            writer.Write(new char[] { 'M', 'T', 'h', 'd' }, 0, 4);
            // write chunk size
            WriteInt32(writer.BaseStream, (uint)0x00000006);
            // write format
            WriteInt16(writer.BaseStream, (ushort)MIDI_FORMAT);

            // write nr of tracks
            WriteInt16(writer.BaseStream, (ushort)NR_OF_TRACKS_USED);

            // write time division
            //WriteVarLen((ulong)TICKS_PER_BEAT, writer.BaseStream);
            WriteInt16(writer.BaseStream, (ushort)TICKS_PER_BEAT);

        }

        private static void WriteTrack(BinaryWriter writer, int instrumentIdx, List<Note> notes)
        {
            // write track chunk ID
            writer.Write(new char[] { 'M', 'T', 'r', 'k' }, 0, 4);


            DateTime start = notes.Min(n => n.TimeDown);
            using (MemoryStream memStream = new MemoryStream())
            {

                // write program change
                byte eventTypeAndChannel = (byte)((MSG_PROGRAM_CHANGE << 4) +  NoteController.MIDI_CHANNEL);
                memStream.WriteByte(0);
                memStream.WriteByte(eventTypeAndChannel);
                memStream.WriteByte((byte)instrumentIdx);

                List<MidiMessage> notesByTimestamp = new List<MidiMessage>();
                foreach (Note n in notes)
                {
                    TimeSpan timeStart = (n.TimeDown - start);
                    TimeSpan timeEnd = timeStart.Add(TimeSpan.FromMilliseconds(n.DurationMS));

                    notesByTimestamp.Add(new MidiMessage()
                    {
                        Time = timeStart,
                        
                        Channel = NoteController.MIDI_CHANNEL,
                        Message = MSG_NOTE_DOWN,
                        Param1 = (byte)n.MidiIndex,
                        Param2 = 64 // velocity
                    });

                    notesByTimestamp.Add(new MidiMessage()
                    {
                        Time = timeEnd,
                        
                        Channel = NoteController.MIDI_CHANNEL,
                        Message = MSG_NOTE_UP,
                        Param1 = (byte)n.MidiIndex,
                        Param2 = 64 // velocity
                    });
                }



                TimeSpan last = new TimeSpan();

                foreach (MidiMessage msg in notesByTimestamp.OrderBy(m => m.Time))
                {
                    // relative delta
                    ulong delta = GetTicks(msg.Time.Subtract(last));
                    last = msg.Time;

                    WriteVarLen(delta, memStream);


                    // c c c c m m m m 
                    byte channelAndMessage = (byte)((msg.Message << 4) + msg.Channel);
                    memStream.WriteByte(channelAndMessage);

                    //WriteInt16(memStream, (ushort)msg.Channel);
                    //WriteInt16(memStream, (ushort)msg.Message);
                    memStream.WriteByte(msg.Param1);
                    memStream.WriteByte(msg.Param2);
                }

                byte[] trackBytes = memStream.ToArray();

                // write track chunk size
                WriteInt32(writer.BaseStream, (uint)(trackBytes.Length));
                
                writer.Write(trackBytes, 0, trackBytes.Length);
            }
        }

        private static void WriteInt32(Stream s, uint val)
        {
            byte[] b= BitConverter.GetBytes(val);
            s.WriteByte(b[3]);
            s.WriteByte(b[2]);
            s.WriteByte(b[1]);
            s.WriteByte(b[0]);
        }

        private static void WriteInt16(Stream s, ushort val)
        {
            byte[] b = BitConverter.GetBytes(val);
            s.WriteByte(b[1]);
            s.WriteByte(b[0]);
        }

        private class MidiMessage
        {
            public TimeSpan Time { get; set; }

            public ulong DeltaTime { get; set; }
            public byte Message { get; set; }
            public byte Channel { get; set; }
            public byte Param1 { get; set; }
            public byte Param2 { get; set; }
        }


        public static int GetNoteDown(short idx, short channel, short velocity)
        {
            return 0x90 + ((5 + (idx + 1)) * 0x100) + (velocity * 0x10000) + channel;
        }

        public static int GetNoteUp(short idx, short channel, short velocity)
        {
            return 0x80 + ((5 + (idx + 1)) * 0x100) + (velocity * 0x10000) + channel;
        }

        private static void WriteVarLen(ulong value, Stream s)
        {
            ulong buffer = value & 0x7F;

            while ((value >>= 7) != 0)
            {
                buffer <<= 8;
                buffer |= ((value & 0x7F) | 0x80);
            }

            while (true)
            {
                s.WriteByte((byte)buffer);
                //putc(buffer,outfile);
                if ((buffer & 0x80) != 0)
                    buffer >>= 8;
                else
                    break;
            }
        }
    }
}
