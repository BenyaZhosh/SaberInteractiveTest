using System;
using System.IO;
using System.Text;

namespace Client
{
    public class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream s)
        {
            ListNode[] arrayRand = new ListNode[Count];
            var currentNode = Head;
            for (int index = 0; index < Count && currentNode != null; index++)
            {
                arrayRand[index] = currentNode;
                currentNode = currentNode.Next;
            }
            
            byte[] buffer = BitConverter.GetBytes(arrayRand.Length);
            s.Write(buffer, 0, buffer.Length);

            for (int i = 0; i < arrayRand.Length; i++)
            {
                var node = arrayRand[i];
                
                buffer = BitConverter.GetBytes(node.Data.Length);
                s.Write(buffer, 0, buffer.Length);
                
                buffer = Encoding.Default.GetBytes(node.Data);
                s.Write(buffer, 0, buffer.Length);

                int randIndex = -1;
                for (int j = 0; j < arrayRand.Length; j++)
                {
                    if (arrayRand[j] == node.Rand)
                    {
                        randIndex = j;
                        break;
                    }
                }
                
                buffer = BitConverter.GetBytes(randIndex);
                s.Write(buffer, 0, buffer.Length);
            }
        }

        public void Deserialize(FileStream s)
        {
            byte[] buffer = new byte[sizeof(int)];
            s.Read(buffer, 0, buffer.Length);
            Count = BitConverter.ToInt32(buffer, 0);
            
            ListNode prevNode = null;

            ListNode[] arrayRand = new ListNode[Count];
            int[] randIndexes = new int[Count];

            for (int i = 0; i < Count; i++)
            {
                buffer = new byte[sizeof(int)];
                s.Read(buffer, 0, buffer.Length);
                int dataLength = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[dataLength];
                s.Read(buffer, 0, buffer.Length);
                string data = "";
                for (int j = 0; j < buffer.Length; j++)
                    data += (char)buffer[j];
                
                buffer = new byte[sizeof(int)];
                s.Read(buffer, 0, buffer.Length);
                randIndexes[i] = BitConverter.ToInt32(buffer, 0);

                ListNode newNode = new ListNode() { Data = data };
                arrayRand[i] = newNode;
                if (i == 0)
                {
                    Head = newNode;
                }
                else if (prevNode != null)
                {
                    prevNode.Next = newNode;
                    newNode.Prev = prevNode;
                }

                if (i == Count - 1)
                {
                    Tail = newNode;
                }
                
                prevNode = newNode;
            }

            ListNode node = Head;
            for (int i = 0; i < Count; i++)
            {
                if (randIndexes[i] >= 0 && randIndexes[i] < Count)
                    node.Rand = arrayRand[randIndexes[i]];
                node = node.Next;
            }
        }
    }
}
