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
        /// <summary>
        /// Use midi format 0, there's only 1 track
        /// </summary>
        private const Int16 MIDI_FORMAT = 0x00;

        /// <summary>
        /// Number of tracks used
        /// </summary>
        private const Int16 NR_OF_TRACKS_USED = 0x01;

        /// <summary>
        /// Milliseconds per minute
        /// </summary>
        private const int MILLISECONDS_PER_MINUTE = 60000;

        /// <summary>
        /// Use 120 bpm
        /// </summary>
        private const int BEATS_PER_MINUTE = 120;

        /// <summary>
        /// Ticks per beat, use 240
        /// </summary>
        private const int TICKS_PER_BEAT = 240;

        /// <summary>
        /// Note down message
        /// </summary>
        private const byte MSG_NOTE_DOWN = 0x9;
        /// <summary>
        /// Note up message
        /// </summary>
        private const byte MSG_NOTE_UP = 0x8;

        /// <summary>
        /// Program change message
        /// </summary>
        private const byte MSG_PROGRAM_CHANGE = 0xC;

        /// <summary>
        /// Return the amount of ticks in a timespan
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static ulong GetTicks(TimeSpan t)
        {
            const double BEAT_PER_MILLISECOND = ((double)BEATS_PER_MINUTE / (double)MILLISECONDS_PER_MINUTE);
            const double TICKS_PER_MILLISECOND = TICKS_PER_BEAT * BEAT_PER_MILLISECOND;

            return (ulong)(t.TotalMilliseconds * TICKS_PER_MILLISECOND);
        }

        /// <summary>
        /// Write a midi file to the given path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="instrumentIdx"></param>
        /// <param name="notes"></param>
        public static void Write(string path, int instrumentIdx,  List<Note> notes)
        {
            using (Stream stream = File.Create(path))
            {
                BinaryWriter writer = new BinaryWriter(stream);
                WriteHeader(writer);

                WriteTrack(writer, instrumentIdx, notes);
            }
        }

        /// <summary>
        /// Write the midi header
        /// </summary>
        /// <param name="writer"></param>
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

        /// <summary>
        /// Write the midi track
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="instrumentIdx"></param>
        /// <param name="notes"></param>
        private static void WriteTrack(BinaryWriter writer, int instrumentIdx, List<Note> notes)
        {
            // write track chunk ID
            writer.Write(new char[] { 'M', 'T', 'r', 'k' }, 0, 4);

            // use first note as start
            DateTime start = notes.Min(n => n.TimeDown);


            using (MemoryStream memStream = new MemoryStream())
            {
                // write program change
                byte eventTypeAndChannel = (byte)((MSG_PROGRAM_CHANGE << 4) +  NoteController.MIDI_CHANNEL);
                memStream.WriteByte(0);
                memStream.WriteByte(eventTypeAndChannel);
                memStream.WriteByte((byte)instrumentIdx);

                // build midi messages from the notes
                List<MidiMessage> midiMessagesFromNotes = new List<MidiMessage>();
                foreach (Note n in notes)
                {
                    TimeSpan timeStart = (n.TimeDown - start);
                    TimeSpan timeEnd = timeStart.Add(TimeSpan.FromMilliseconds(n.DurationMS));

                    midiMessagesFromNotes.Add(new MidiMessage()
                    {
                        Time = timeStart,
                        
                        Channel = NoteController.MIDI_CHANNEL,
                        Message = MSG_NOTE_DOWN,
                        Param1 = (byte)n.MidiIndex,
                        Param2 = 64 // velocity
                    });

                    midiMessagesFromNotes.Add(new MidiMessage()
                    {
                        Time = timeEnd,
                        
                        Channel = NoteController.MIDI_CHANNEL,
                        Message = MSG_NOTE_UP,
                        Param1 = (byte)n.MidiIndex,
                        Param2 = 64 // velocity
                    });
                }

                TimeSpan last = new TimeSpan();
                foreach (MidiMessage msg in midiMessagesFromNotes.OrderBy(m => m.Time))
                {
                    // relative delta
                    ulong delta = GetTicks(msg.Time.Subtract(last));
                    last = msg.Time;

                    WriteVarLen(delta, memStream);

                    // write channel and midi message
                    byte channelAndMessage = (byte)((msg.Message << 4) + msg.Channel);
                    memStream.WriteByte(channelAndMessage);

                    // write midi index & velocity
                    memStream.WriteByte(msg.Param1);
                    memStream.WriteByte(msg.Param2);
                }

                // get bytes from total track
                byte[] trackBytes = memStream.ToArray();

                // write track chunk size
                WriteInt32(writer.BaseStream, (uint)(trackBytes.Length));
                
                // write track
                writer.Write(trackBytes, 0, trackBytes.Length);
            }
        }

        /// <summary>
        /// Write int in big endian to the given stream
        /// </summary>
        /// <param name="s"></param>
        /// <param name="val"></param>
        private static void WriteInt32(Stream s, uint val)
        {
            byte[] b= BitConverter.GetBytes(val);
            s.WriteByte(b[3]);
            s.WriteByte(b[2]);
            s.WriteByte(b[1]);
            s.WriteByte(b[0]);
        }

        /// <summary>
        /// Write short in big endian to the given stream
        /// </summary>
        /// <param name="s"></param>
        /// <param name="val"></param>
        private static void WriteInt16(Stream s, ushort val)
        {
            byte[] b = BitConverter.GetBytes(val);
            s.WriteByte(b[1]);
            s.WriteByte(b[0]);
        }

        /// <summary>
        /// A midi message for a midi track
        /// </summary>
        private class MidiMessage
        {
            public TimeSpan Time { get; set; }

            public ulong DeltaTime { get; set; }
            public byte Message { get; set; }
            public byte Channel { get; set; }
            public byte Param1 { get; set; }
            public byte Param2 { get; set; }
        }

        /// <summary>
        /// Write given long as variable length
        /// </summary>
        /// <param name="value"></param>
        /// <param name="s"></param>
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
