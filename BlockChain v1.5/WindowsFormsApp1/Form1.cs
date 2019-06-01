using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Security.Cryptography;

namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            //Random rnd = new Random(DateTime.UtcNow.Millisecond);
            //IBlock genesis = new Block(new byte[] { 0x00, 0x00, 0x00 });
            //byte[] difficulty = new byte[] { 0x00, 0x00 };

            //BlockChain chain = new BlockChain(genesis);
            //for (int i = 0; i < 1; i++)
            {
                //    string input = "21354";
                //    byte[] data = System.Text.Encoding.UTF8.GetBytes(input);
                //    //var data = Enumerable.Range(0, 2256).Select(p => (byte)rnd.Next());
                //    chain.Add(new Block(data.ToArray()));
                //    using (StreamWriter stream = File.AppendText("BlockChain.txt"))
                //    {
                //        stream.WriteLine(chain.LastOrDefault()?.ToString());
                //    }

                var fileStream = new FileStream("BlockChain.txt", FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] words = line.Split();
                    }
                }

            //    //Console.Write(chain.LastOrDefault()?.ToString());
            //    textBox1.AppendText(chain.LastOrDefault()?.ToString());
            //    //Console.WriteLine($"Chain is valid: {chain.IsValid()}");
            //    textBox1.AppendText($"Chain is valid: {chain.IsValid()}");
        }

    }
    }

    public interface IBlock
    {
        byte[] Data { get; }
        byte[] Hash { get; set; }
        byte[] PrevHash { get; set; }
        DateTime TimeStamp { get; }
    }

    public static class BlockChainExtension
    {
        public static byte[] GenerateHash(this IBlock block)
        {
            using (SHA512 sha = new SHA512Managed())
            using (MemoryStream st = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(st))
            {
                bw.Write(block.Data);
                bw.Write(block.TimeStamp.ToBinary());
                bw.Write(block.PrevHash);
                var starr = st.ToArray();
                return sha.ComputeHash(starr);
            }
        }
        public static byte[] MineHash(this IBlock block/*, byte[] difficulty*/)
        {
            byte[] hash = new byte[0];           {
            hash = block.GenerateHash();            }
            return hash;
        }
        public static bool IsValid(this IBlock block)
        {
            var bk = block.GenerateHash();
            return block.Hash.SequenceEqual(bk);
        }
        public static bool IsValidPrevBlock(this IBlock block, IBlock prevBlock)
        {
            if (prevBlock == null) throw new ArgumentNullException(nameof(prevBlock));

            var prev = prevBlock.GenerateHash();
            return prevBlock.IsValid() && block.PrevHash.SequenceEqual(prev);
        }
        public static bool IsValid(this IEnumerable<IBlock> items)
        {
            var enumerable = items.ToList();
            return enumerable.Zip(enumerable.Skip(1), Tuple.Create).All(block => block.Item2.IsValid() && block.Item2.IsValidPrevBlock(block.Item1));
        }
    }

    public class Block : IBlock
    {
        public Block(byte[] data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            //Nonce = 0;
            PrevHash = new byte[] { 0x00 };
            TimeStamp = DateTime.Now;
        }

        public byte[] Data { get; }
        public byte[] Hash { get; set; }
        //public int Nonce { get; set; }
        public byte[] PrevHash { get; set; }
        public DateTime TimeStamp { get; }

        public override string ToString()
        {
            return $"{BitConverter.ToString(Hash).Replace("-", "")}\n{BitConverter.ToString(PrevHash).Replace("-", "")}\n {BitConverter.ToString(Data)}\n {TimeStamp}";
        }

        
    }

    public class BlockChain : IEnumerable<IBlock>
    {
        private List<IBlock> _items = new List<IBlock>();

        public BlockChain(/*byte[] difficulty*/ IBlock genesis)
        {
            //Difficulty = difficulty;
            genesis.Hash = genesis.MineHash();
            Items.Add(genesis);
        }

        public void Add(IBlock item)
        {
            if (Items.LastOrDefault() != null)
            {
                item.PrevHash = Items.LastOrDefault()?.Hash;
            }
            item.Hash = item.MineHash();
            Items.Add(item);
        }

        public int Count => Items.Count;
        public IBlock this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }

        public List<IBlock> Items
        {
            get => _items;
            set => _items = value;
        }

        public byte[] Difficulty { get; }

        public IEnumerator<IBlock> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }


}
