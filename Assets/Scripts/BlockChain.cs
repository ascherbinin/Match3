using UnityEngine;
using System.Collections;


namespace BlockNS
{

    public class BlockChain
    {

        public enum ChainType
        {
            ChainTypeHorizontal,
            ChainTypeVertical
        }

        ArrayList _blocks;

        public void AddBlock(Block block)
        {
            if (block == null)
            {
                _blocks = new ArrayList();
            }
            _blocks.Add(block);
        }

        public ArrayList Blocks()
        {
            return _blocks;
        }
      
    }
}