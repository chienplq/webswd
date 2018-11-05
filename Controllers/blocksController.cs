using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace demoWebAdmin.Controllers
{
    public class blocksController : Controller
    {
        private blockChainEntities db = new blockChainEntities();
        private List<block> listBlock = null;



        // GET: blocks
        public ActionResult Index()
        {

            return View(db.blocks.ToList());
        }

        // GET: blocks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            block block = db.blocks.Find(id);

            if (block == null)
            {
                listBlock = db.blocks.ToList();
                return HttpNotFound();
            }
            return View(block);
        }

        // GET: blocks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: blocks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "index,previousHash,hash,timestamp,data")] block block)
        {
            block.index = getLastBlock().index + 1;
            block.previousHash = getLastBlock().hash;
            block.timestamp = DateTime.Now;
            block.hash = calculateHash(block);
            if (addNewBlock(block))
            {
                if (ModelState.IsValid)
                {
                    db.blocks.Add(block);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(block);
            }
            else return View(block);



        }

        // GET: blocks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            block block = db.blocks.Find(id);
            if (block == null)
            {
                return HttpNotFound();
            }
            return View(block);
        }

        // POST: blocks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "index,previousHash,hash,timestamp,data,status")] block block)
        {

            db.Entry(block).State = EntityState.Modified;
            listBlock = db.blocks.ToList();

            if (ModelState.IsValid)
            {
                listBlock = db.blocks.ToList();
                block.timestamp = DateTime.Now;
                block.hash = calculateHash(block);
                if (isValidBlockChain(listBlock, listBlock[listBlock.Count - 1].hash) != -1)
                {
                    for (int i = block.index; i <= listBlock.Count(); i++)
                    {
                        block bl = db.blocks.Find(i);
                        if (bl != null)
                        {
                            db.Entry(bl).State = EntityState.Modified;
                            bl.status = false;
                        }
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(block);
        }

        // GET: blocks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            block block = db.blocks.Find(id);
            if (block == null)
            {
                return HttpNotFound();
            }
            return View(block);
        }

        // POST: blocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            listBlock = db.blocks.ToList();
            if (isValidBlockChain(listBlock, listBlock[listBlock.Count - 1].hash) != -1)
            {
                for (int i = id; i <= listBlock.Count(); i++)
                {
                    block bl = db.blocks.Find(i + 1);
                    if (bl != null)
                    {
                        db.Entry(bl).State = EntityState.Modified;
                        bl.status = false;
                    }
                }
            }
            block block = db.blocks.Find(id);
            db.blocks.Remove(block);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public string calculateHash(block bl)
        {
            var input = bl.index + bl.previousHash + bl.timestamp.ToString() + bl.data + "";
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
        public block getGenesisBlock()
        {
            block bl = new block();
            bl.index = 0;
            bl.previousHash = "0";
            bl.timestamp = DateTime.Now;
            bl.data = "This is the Genesis block.";
            bl.hash = calculateHash(bl);
            return bl;
        }
        public bool isValidNewBlock(block previousBlock, block newBlock)
        {
            if (previousBlock.index + 1 != newBlock.index)
            {

                return false;
            }
            else if (previousBlock.hash != newBlock.previousHash)
            {

                return false;
            }
            else if (calculateHash(newBlock) != newBlock.hash)
            {

                return false;
            }


            return true;
        }
        public block getLastBlock()
        {

            listBlock = db.blocks.ToList();

            return listBlock[listBlock.Count - 1];

        }
        public bool addNewBlock(block block)
        {

            if (isValidNewBlock(getLastBlock(), block))
            {

                listBlock.Add(block);
                return true;
            }
            return false;
        }
        public int isValidBlockChain(List<block> listBl, string lastHash)
        {
            if (listBl[0].index != 0 || !listBl[0].previousHash.Equals('0')
                || calculateHash(listBl[0]).Equals(listBl[0].hash))
            {
                return 0;
            }
            for (int i = 1; i < listBl.Count - 1; i++)
            {
                if (listBl[i].index != i) return i;
                if (!listBl[i].previousHash.Equals(listBl[i - 1].hash)) return (i - 1);
                if (listBl[i].hash.Equals(calculateHash(listBl[i]))) return i;
            }
            if (!listBlock[listBlock.Count - 1].hash.Equals(lastHash)) { return (listBlock.Count - 1); }

            return -1;
        }
    }
}
