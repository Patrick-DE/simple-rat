using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SimpleRAT
{
    public class Intro
    {
        private string introText;
        private Random random = new Random(Environment.TickCount);

        private int GetRandomInt(){
            random = new Random(random.Next() ^ Environment.TickCount);
            return random.Next();
        }

        private long GenerateRandom() {
            return ((long)GetRandomInt()) << 32 | GetRandomInt();
        }

        private string GenerateRandomHexAscii16(long number) {
            return $"{HexToString(number)} {HexToAscii(number)}";
        }

        private string HexToString(long number) {
            return number.ToString("X16").PadLeft(16, '0');
        }

        private string HexToAscii(long number) {
            return string.Join("", BitConverter.GetBytes(number).Select(x => (x >= 'a' && x <= 'z')
                                                                        || (x >= 'A' && x <= 'Z')
                                                                        || (x >= '0' && x <= '9') ? (char)x : '.').ToArray());
        }

        public Intro()
        {
            using (var str = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("INTRO.txt"))
            {
                using (var reader = new StreamReader(str))
                {
                    introText = reader.ReadToEnd()
                                      .Replace("{RANDOM_HEX}", "0x" + HexToString(GenerateRandom()))
                                      .Replace("{RANDOM_HEX_ASCII_16}", GenerateRandomHexAscii16(GenerateRandom()))
                                      .Replace("{RANDOM_HEX_128}", 
                                               HexToString(GenerateRandom()) + 
                                               HexToString(GenerateRandom()) + 
                                               HexToString(GenerateRandom()) + 
                                               HexToString(GenerateRandom()));
                }
            }
        }

        public void Play(int sleep) {
            foreach (var c in introText) {
                Console.Write(c);
                Thread.Sleep(sleep);
            }
        }
    }
}
