using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace seqfind
{
    class Program
    {
        static void Main(string[] args)
        {
            int length = 40;
            string fasta_file1 = null;
            string fasta_file2 = null;
            string output_file = null;

            if(args.Length!=4)
            {
                Console.WriteLine("Syntax:");
                Console.WriteLine("\tseqfind.exe <length> <fasta1/seq1> <fasta2/seq2> <output-file>");
                return;
            }
            else
            {
                //
                length = int.Parse(args[0]);
                fasta_file1 = args[1];
                fasta_file2 = args[2];
                output_file = args[3];
            }

            Console.WriteLine("Processing ..." + fasta_file1);
            StreamReader reader = new StreamReader(File.OpenRead(fasta_file1));
            string line = null;
            StringBuilder sb = new StringBuilder();
            Dictionary<string, int> key = new Dictionary<string, int>();
            string tmp = "";
            int pos = 0;
            if (fasta_file1.EndsWith(".fa")||fasta_file1.EndsWith(".fasta"))
                line = reader.ReadLine();// ignore first fasta line.
            while ((line = reader.ReadLine()) != null)
            {
                foreach (char c in line.Trim().ToCharArray())
                {
                    pos++;

                    sb.Append(c.ToString());
                    if (sb.Length == length)
                    {
                        tmp = sb.ToString().ToUpper();
                        if (!key.ContainsKey(tmp))
                        {
                            if (tmp.IndexOf('N') == -1)
                                key.Add(tmp, pos - length);
                        }
                        sb.Clear();
                        if (tmp != "")
                            sb.Append(tmp.Substring(1));
                    }
                }
            }
            reader.Close();
            Console.WriteLine("Total Sequences:" + key.Count);

            Console.WriteLine("Processing ..." + fasta_file2);
            reader = new StreamReader(File.OpenRead(fasta_file2));
            //StreamWriter writer = new StreamWriter(File.OpenWrite(output_file));
            if (fasta_file2.EndsWith(".fa") || fasta_file2.EndsWith(".fasta"))
                line = reader.ReadLine();// ignore first fasta line.
            sb.Clear();
            pos = 0;
            string file1 = Path.GetFileName(fasta_file1);
            string file2 = Path.GetFileName(fasta_file2);
            StringBuilder out_file = new StringBuilder();
            while((line = reader.ReadLine())!=null)
            {
                foreach (char c in line.Trim().ToCharArray())
                {
                    pos++;

                    sb.Append(c.ToString());
                    if (sb.Length == length)
                    {
                        tmp = sb.ToString().ToUpper();
                        if (key.ContainsKey(tmp))
                        {
                            //writer.WriteLine(file1 + " = " + key[tmp] + ", " + file2 + " = " + (pos - length) + ", Sequence = " + tmp);
                            out_file.Append(key[tmp] + "," + (pos - length) + "," + tmp+"\r\n");
                            key.Remove(tmp);
                            sb.Clear();
                            continue;
                        }
                        else if (key.ContainsKey(Reverse(tmp)))
                        {
                            //writer.WriteLine(file1 + " = " + key[Reverse(tmp)] + ", " + file2 + " = " + (pos - length) + ", Sequence[R] = " + Reverse(tmp));
                            out_file.Append(key[Reverse(tmp)] + "," + (pos - length) + "," + Reverse(tmp)+"\r\n");
                            key.Remove(Reverse(tmp));
                            sb.Clear();
                            continue;
                        }
                        //

                        
                        sb.Clear();
                        if (tmp!="")
                            sb.Append(tmp.Substring(1));                         
                    }                    
                }
            }
            reader.Close();
            //writer.Close();
            if(out_file.ToString()!="")
            {
                File.WriteAllText(output_file, out_file.ToString());
            }
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
