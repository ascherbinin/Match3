using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace BlockNS
{
    public enum ChainType
    {
        ChainTypeHorizontal,
        ChainTypeVertical
    }

    public class BlockChain
    {

        

        List<Block> _blocks;

        public void AddBlock(Block block)
        {
            if (_blocks == null)
            {
                _blocks = new List<Block>();
            }
            _blocks.Add(block);
        }

        public List<Block> Blocks()
        {
            return _blocks;
        }

        public int Length()
        {
            return _blocks.Count;
        }

        public ChainType ChainType { get; set; }
    }
}