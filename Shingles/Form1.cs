using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace Shingles
{
    public partial class Form1 : Form
    {
        List<string> separators;
        List<string> words;
        int shingleLen = 10;
        int startHash = 10;
        int numHash = 84;
        public Form1()
        {
            InitializeComponent();
            separators = new List<string>{ ".",",",";",":","!","?", "-" };
           // words = new List<string> { "на", "так", "как", "в", "" };
            //words = new List<string>(new string[] {""});
        }

        private void button1_Click(object sender, EventArgs e)
        {

            String temp = richTextBox1.Text;
            temp = temp.ToLower();
            foreach (string separator in separators)
            {
                temp = temp.Replace(separator, "");
            }

            List<String> words = SplitWords(temp);
            List<String> shingles = CreateShingles(words);
            List<List<Int64>> hashes = GetHashes(shingles);
            //for (int i = 0; i < shingles.Count; i++ )
            //    richTextBox2.Text += shingles[i] +" " + hashes[i] + "\n";


            String temp2 = richTextBox4.Text;
            temp2 = temp2.ToLower();
            foreach (string separator in separators)
            {
                temp2 = temp2.Replace(separator, "");
            }

            List<String> words2 = SplitWords(temp2);
            List<String> shingles2 = CreateShingles(words2);
            List<List<Int64>> hashes2 = GetHashes(shingles2);
            //for (int i = 0; i < shingles2.Count; i++)
            //    richTextBox3.Text += shingles2[i] + " " + hashes2[i] + "\n";


            List<Int64> min1 = GetHashMinimums(hashes);//CompareHashes(hashes, hashes2)*100;
            List<Int64> min2 = GetHashMinimums(hashes2);//CompareHashes(hashes2, hashes)*100;

            double res1 = CompareHashes(min1,min2);

            label2.Text = (res1*100).ToString()+"%";
        }

        private List<String> SplitWords(String Text)
        {
            List<String> result = new List<string>(Text.Split(' '));
            
            return result;
        }

        private List<String> CreateShingles(List<String> words)
        {
            List<String> result = new List<String>();

            for (int i = 0; i < words.Count - shingleLen + 1; i++)
            {
                result.Add("");
                for (int j = i; j < i+shingleLen; j++)
                { 
                    result[i] += words[j] + " ";
                }
                result[i].Trim();
            }
           
            return result;
        }

        private Int64 PolinomialHash(int p, string str)
        {
            Int64 res = 0;
            Int64 ppow = 1;

            for (int i = 0; i < str.Length; i++)
            {
                res += str[i] * ppow;
                ppow *= p;
            }


            return res;
        }

        private List<List<Int64>> GetHashes(List<String> shingles)
        {
            List<List<Int64>> result = new List<List<Int64>>();
            List<Int64> tmp = new List<Int64>();
            MD5 md5 = MD5.Create();

            foreach(String shingle in shingles)
            {
                tmp = new List<Int64>();
                for (int i = startHash; i < startHash + numHash; i++)
                {
                    tmp.Add(Math.Abs(PolinomialHash(i, shingle)));
                }
                result.Add(tmp);
            }

            return result;
        }

        private List<Int64> GetHashMinimums(List<List<Int64>> inc)
        {
            List<Int64> min = new List<Int64>();

            min = inc[0];

            for (int i = 1; i < inc.Count; i++)
            {
                for (int j = 0; j < numHash; j++)
                {
                    if (inc[i][j] < min[j])
                        min[j] = inc[i][j];
                }
            }

                return min;
        }

        private double CompareHashes(List<Int64> orig, List<Int64> suspect)
        {
            double result;
            int acc =  0;

            for (int i = 0; i < orig.Count; i++)
            {
                if (orig[i] == suspect[i])
                    acc++;    
            }

            result = (double)(acc) / (double)(orig.Count);

            return result;
        }

        private List<List<Int64>> GetSuperShingles(List<Int64> mins)
        {
            List<List<Int64>> res = new List<List<Int64>>();
            List<Int64> tmp;

            for (int i = 0; i < mins.Count; i++)
            {
                if (i % 6 == 0)
                {
                    tmp = new List<Int64>();
                    res.Add(tmp);
                }
                res[i / 6].Add(mins[i]);
            }
             return res;
        }

        private double CompareSuperShingles(List<List<Int64>> inc1, List<List<Int64>> inc2)
        {
            double res = 0;
            int acc = 0;

            for (int i = 0; i < inc1.Count; i++)
            {
                if (CompareLists(inc1[i], inc2[i]) == 1)
                    acc++;
            }
            res = (double)(acc) / (double)(inc1.Count);
                return res;
        }

        private int CompareLists(List<Int64> l1, List<Int64> l2)
        {
            for (int i = 0; i < l1.Count; i++)
            {
                if (l1[i] != l2[i])
                    return 0;
            }
            return 1;
        }

        private List<List<Int64>> GetMegaShingles(List<List<Int64>> supSh)
        {
            List<List<Int64>> res = new List<List<Int64>>();
            List<Int64> tmp = new List<long>();

            for (int i = 0; i < supSh.Count; i++)
            {
                for (int j = i + 1; j < supSh.Count; j++)
                {
                    tmp = supSh[i];
                    tmp.Concat(supSh[j]);
                    res.Add(tmp);
                }
            }
            return res;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            String temp = richTextBox1.Text;
            temp = temp.ToLower();
            foreach (string separator in separators)
            {
                temp = temp.Replace(separator, "");
            }

            List<String> words = SplitWords(temp);
            List<String> shingles = CreateShingles(words);
            List<List<Int64>> hashes = GetHashes(shingles);
            //for (int i = 0; i < shingles.Count; i++ )
            //    richTextBox2.Text += shingles[i] +" " + hashes[i] + "\n";


            String temp2 = richTextBox4.Text;
            temp2 = temp2.ToLower();
            foreach (string separator in separators)
            {
                temp2 = temp2.Replace(separator, "");
            }

            List<String> words2 = SplitWords(temp2);
            List<String> shingles2 = CreateShingles(words2);
            List<List<Int64>> hashes2 = GetHashes(shingles2);
            //for (int i = 0; i < shingles2.Count; i++)
            //    richTextBox3.Text += shingles2[i] + " " + hashes2[i] + "\n";


            List<Int64> min1 = GetHashMinimums(hashes);//CompareHashes(hashes, hashes2)*100;
            List<Int64> min2 = GetHashMinimums(hashes2);//CompareHashes(hashes2, hashes)*100;

            List<List<Int64>> sup1 = GetSuperShingles(min1);
            List<List<Int64>> sup2 = GetSuperShingles(min2);
            //double res1 = CompareHashes(min1, min2);
            double res = CompareSuperShingles(sup1, sup2);
            label2.Text = (res * 100).ToString() + "%";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String temp = richTextBox1.Text;
            temp = temp.ToLower();
            foreach (string separator in separators)
            {
                temp = temp.Replace(separator, "");
            }

            List<String> words = SplitWords(temp);
            List<String> shingles = CreateShingles(words);
            List<List<Int64>> hashes = GetHashes(shingles);
            //for (int i = 0; i < shingles.Count; i++ )
            //    richTextBox2.Text += shingles[i] +" " + hashes[i] + "\n";


            String temp2 = richTextBox4.Text;
            temp2 = temp2.ToLower();
            foreach (string separator in separators)
            {
                temp2 = temp2.Replace(separator, "");
            }

            List<String> words2 = SplitWords(temp2);
            List<String> shingles2 = CreateShingles(words2);
            List<List<Int64>> hashes2 = GetHashes(shingles2);
            //for (int i = 0; i < shingles2.Count; i++)
            //    richTextBox3.Text += shingles2[i] + " " + hashes2[i] + "\n";


            List<Int64> min1 = GetHashMinimums(hashes);//CompareHashes(hashes, hashes2)*100;
            List<Int64> min2 = GetHashMinimums(hashes2);//CompareHashes(hashes2, hashes)*100;

            List<List<Int64>> sup1 = GetSuperShingles(min1);
            List<List<Int64>> sup2 = GetSuperShingles(min2);
            //double res1 = CompareHashes(min1, min2);
            List<List<Int64>> megaSh1 = GetMegaShingles(sup1);
            List<List<Int64>> megaSh2 = GetMegaShingles(sup2);


            double res = CompareSuperShingles(megaSh1, megaSh2);
            label2.Text = (res * 100).ToString() + "%";
        }

        
    }
}
