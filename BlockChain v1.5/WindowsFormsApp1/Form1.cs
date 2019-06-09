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

namespace WindowsFormsApp1
{
 
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        Sync s = new Sync();

        private void button1_Click(object sender, EventArgs e)
        {
            IBlock genesis = new Block(new byte[] { 0x00, 0x00, 0x00 });
            BlockChain newChain = new BlockChain(genesis); //keeps creating new chain so data is lost after new vote need fixing

            if (radioButton1.Checked == true)
            {
                if (radioButton3.Checked == true)
                {
                    textBox1.AppendText(s.Add(1,1));
                }
                else if (radioButton4.Checked == true)
                {

                    textBox1.AppendText(s.Add(1,2));
                }
                else { }

            }
            else
            if (radioButton2.Checked == true)
            {
                if (radioButton3.Checked == true)
                {
                    textBox1.AppendText(s.Add(2,1));
                }
                else if (radioButton4.Checked == true)
                {
                    textBox1.AppendText(s.Add(2,2));
                }
                else { }
            }
            else
            {

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int count = Convert.ToInt32(numericUpDown1.Value);
            byte[] hashchange = new byte[] { 0x3 };
            s.insert(count, hashchange);
        }
    }

    public interface IBlock
    {
        byte[] Data { get; set; }
        byte[] Hash { get; set; }
        byte[] PrevHash { get; set; }
        DateTime TimeStamp { get; set; }
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
            byte[] hash = new byte[0];
            {
                hash = block.GenerateHash();
            }
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

        public byte[] Data { get; set; }
        public byte[] Hash { get; set; }
        //public int Nonce { get; set; }
        public byte[] PrevHash { get; set; }
        public DateTime TimeStamp { get; set; }

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

    public class Sync
    {
        node1 n1 = new node1();
        node2 n2 = new node2();

        IBlock genesis = new Block(new byte[] { 0x00, 0x00, 0x00 });
        BlockChain newChain;


        public string Add(int choice1,int choice2)
        {
            bool ver;
            string output;
            if(choice1==1)
            {
                newChain = n1.AddBlock(choice2);

                ver = syncronize(1);
                if (ver)
                {
                    output = "verified";
                }
                else
                {
                    output = "not verified";
                }
                return output;
            }
            else
            {
                newChain = n2.AddBlock(choice2);

                ver = syncronize(2);
                if (ver)
                {
                    output = "verified";
                }
                else
                {
                    output = "not verified";
                }
                return output;
            }

        }

        //public string Node1(int choice)
        //{
        //    bool ver;
        //    string output;
        //    newChain = n1.AddBlock(choice);

        //    ver = syncronize(1);
        //    if (ver)
        //    {
        //        output = "verified";
        //    }
        //    else
        //    {
        //        output = "not verified";
        //    }
        //    return output;
        //}

        //public string Node2(int choice)
        //{
        //    bool ver;
        //    string output;
        //    newChain = n2.AddBlock(choice);

        //    ver = syncronize(2);
        //    if(ver)
        //    {
        //        output = "verified";
        //    }
        //    else
        //    {
        //        output = "not verified";
        //    }
        //    return output;
        //}

        public bool syncronize(int nodeNumb)
        {
            if(nodeNumb==1)
            {
                return n2.verify(newChain);
            }
            else
            {
                return n1.verify(newChain);
            }
        }

        public void insert(int count,byte[] BadHash)
        {
            newChain.ElementAt(count).Data=BadHash;
            syncronize(1);
        }
    }

    public class node1
    {
        IBlock genesis = new Block(new byte[] { 0x00, 0x00, 0x00 });
        BlockChain chain;


        public BlockChain AddBlock(int input)
        {
            if (chain == null)
            {
                DateTime myDate = DateTime.ParseExact("2009-05-08 14:40:52,531", "yyyy-MM-dd HH:mm:ss,fff",System.Globalization.CultureInfo.InvariantCulture);
                genesis.TimeStamp = myDate;
                chain = new BlockChain(genesis);
                string choice = input.ToString();
                byte[] data = System.Text.Encoding.UTF8.GetBytes(choice);
                chain.Add(new Block(data.ToArray()));
                return chain;
            }
            else
            {
                string choice = input.ToString();
                byte[] data = System.Text.Encoding.UTF8.GetBytes(choice);
                chain.Add(new Block(data.ToArray()));
                return chain;
            }
        }

        public bool verify(BlockChain ChainCompare)
        {
            bool verified=true;
            int i = 0;
            if (chain == null)
            {
                DateTime myDate = DateTime.ParseExact("2009-05-08 14:40:52,531", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
                genesis.TimeStamp = myDate;
                chain = new BlockChain(genesis);
            }

            foreach (Block item in chain)
            {
                if (item.Hash.SequenceEqual(ChainCompare.ElementAt(i).Hash))
                {

                }
                else
                {
                    verified = false;
                    break;
                }
                i++;
            }
            
            if(verified)
            {
                verified = ChainCompare.IsValid();
                if (verified)
                {
                    chain = ChainCompare;
                }
            }

            return verified;
        }
    }


public class node2
    {

        IBlock genesis = new Block(new byte[] { 0x00, 0x00, 0x00 });
        BlockChain chain;

        public BlockChain AddBlock(int input)
        {
            if(chain == null)
            {
                DateTime myDate = DateTime.ParseExact("2009-05-08 14:40:52,531", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
                genesis.TimeStamp = myDate;
                chain = new BlockChain(genesis);

                string choice = input.ToString();
                byte[] data = System.Text.Encoding.UTF8.GetBytes(choice);
                chain.Add(new Block(data.ToArray()));

                return chain;
            }
            else
            {
                string choice = input.ToString();
                byte[] data = System.Text.Encoding.UTF8.GetBytes(choice);
                return chain;
            }
        }

        public bool verify(BlockChain ChainCompare)
        {
            bool verified = true;
            int i = 0;
            if(chain==null)
            {
                DateTime myDate = DateTime.ParseExact("2009-05-08 14:40:52,531", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
                genesis.TimeStamp = myDate;
                chain = new BlockChain(genesis);
            }

            foreach (Block item in chain)
            {
                if (item.Hash.SequenceEqual(ChainCompare.ElementAt(i).Hash))
                {
                    verified = true;
                }
                else
                {
                    verified = false;
                    break;
                }
                i++;
            }

            if (verified)
            {
                verified = ChainCompare.IsValid();
                if (verified)
                {
                    chain = ChainCompare;
                }
            }

            return verified;
        }
    }
}
