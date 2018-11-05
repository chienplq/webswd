//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace demoWebAdmin
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    public partial class block
    {
        public int index { get; set; }
        public string previousHash { get; set; }
        public string hash { get; set; }
        public Nullable<System.DateTime> timestamp { get; set; }
        public string data { get; set; }
        public Nullable<bool> status { get; set; }

        public void changeData(String dt)
        {
            this.data = dt;
            this.timestamp = DateTime.Now;
            this.hash = calculateHash(this);
        }
        public void show()
        {
            Console.WriteLine("Index: " + index + "\n PreviousHash: " + previousHash + "\n Hash: " + hash + "\n Timestap: "
                + timestamp + "\n Data: " + data);
        }
        public string calculateHash(block bl)
        {
            var input = bl.index + bl.previousHash + bl.timestamp.ToString() + bl.data + "";
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public override bool Equals(object obj)
        {
            var block = obj as block;
            return block != null;
        }

    }
}