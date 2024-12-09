namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;

using matthiasffm.Common.Math;

// TODO: inserting into id list in InsertIdBlock() method is slow O(n)
//       could use a binary tree instead for O(logn) insertion times

/// <summary>
/// Theme: defragmentation
/// </summary>
[TestFixture]
public class Day09
{
    private static IEnumerable<long> ParseData(string data)
        => data.ToCharArray()
               .Select(c => (long)c - 48L);

    [Test]
    public void TestSamples()
    {
        var data   = "2333133121414131402";
        var layout = ParseData(data);

        Puzzle1(layout).Should().Be(1928);
        Puzzle2(layout).Should().Be(2858);
    }

    [Test]
    public void TestAocInput()
    {
        var data     = FileUtils.ReadAllText(this);
        var layout = ParseData(data);

        Puzzle1(layout).Should().Be(6211348208140L);
        Puzzle2(layout).Should().Be(6239783302560L);
    }

    // an amphipod is struggling with his computer. He's trying to make more contiguous free space by compacting all of the files, but his program isn't working. He shows you
    // the disk map (your puzzle input) he's already generated. The disk map uses a dense format to represent the layout of files and free space on the disk. The digits alternate
    // between indicating the length of a file and the length of free space. Each file on disk also has an ID number based on the order of the files as they appear before they
    // are rearranged, starting with ID 0.
    // The amphipod would like to move file blocks one at a time from the end of the disk to the leftmost free space block (until there are no gaps remaining between file blocks).
    // The final step of this file-compacting process is to update the filesystem checksum. To calculate the checksum, add up the result of multiplying each of these blocks'
    // position with the file ID number it contains. The leftmost block is in position 0. If a block contains free space, skip it instead.
    //
    // Puzzle == Compact the amphipod's hard drive using the process he requested. What is the resulting filesystem checksum?
    private static long Puzzle1(IEnumerable<long> layout)
    {
        // split layout into list of ids and list of free blocks

        var (ids, free) = SplitLayout(layout);

        // so long as there are free blocks
        //     take last id block
        //     redistribute this (or a part of it) to first free block and modify id list and free block list in the process
        while(free.Any())
        {
            var lastIdBlock  = ids.Last!;
            var freeToInsert = free.First!;

            if(freeToInsert.Value.Pos > lastIdBlock.Value.Pos)
            {
                // only free memory is past last id block, this is fine
                break;
            }

            if(freeToInsert.Value.Length >= lastIdBlock.Value.Length)
            {
                ids.RemoveLast();
                InsertIdBlock(ids, freeToInsert.Value.Pos, lastIdBlock.Value.ID, lastIdBlock.Value.Length);
                if(freeToInsert.Value.Length == lastIdBlock.Value.Length)
                {
                    free.RemoveFirst();
                }
                else
                {
                    freeToInsert.ValueRef.Length -= lastIdBlock.Value.Length;
                    freeToInsert.ValueRef.Pos += lastIdBlock.Value.Length;
                }
            }
            else
            {
                free.RemoveFirst();
                InsertIdBlock(ids, freeToInsert.Value.Pos, lastIdBlock.Value.ID, freeToInsert.Value.Length);                
                lastIdBlock.ValueRef.Length -= freeToInsert.Value.Length;
            }
        }

        return Checksum(ids);
    }

    // The eager amphipod already has a new plan: rather than move individual blocks, he'd like to try compacting the files on his disk by moving whole files instead.
    // This time, attempt to move whole files to the leftmost span of free space blocks that could fit the file. Attempt to move each file exactly once in order of
    // decreasing file ID number starting with the file with the highest file ID number. If there is no span of free space to the left of a file that is large enough
    // to fit the file, the file does not move.
    //
    // Puzzle == Start over, now compacting the amphipod's hard drive using this new method instead. What is the resulting filesystem checksum?
    private static long Puzzle2(IEnumerable<long> layout)
    {
        // split layout into list of ids and list of free blocks

        var (ids, free) = SplitLayout(layout);

        // try to insert id blocks from last to first
        //     find free mem slot where id fit completely
        //     modify id list and free block list in the process

        var currentIdBlock = ids.Last;
        do
        {
            var freeToInsert = free.First!;

            while(freeToInsert != free.Last && freeToInsert!.Value.Pos < currentIdBlock!.Value.Pos && freeToInsert.Value.Length < currentIdBlock.Value.Length)
            {
                freeToInsert = freeToInsert.Next;
            }

            if(freeToInsert!.Value.Pos < currentIdBlock!.Value.Pos && freeToInsert.Value.Length >= currentIdBlock.Value.Length)
            {
                var blockToRemove = currentIdBlock;
                currentIdBlock = currentIdBlock.Previous;

                ids.Remove(blockToRemove);
                InsertIdBlock(ids, freeToInsert.Value.Pos, blockToRemove.Value.ID, blockToRemove.Value.Length);

                if(freeToInsert.Value.Length == blockToRemove.Value.Length)
                {
                    free.Remove(freeToInsert);
                }
                else
                {
                    freeToInsert.ValueRef.Length -= blockToRemove.Value.Length;
                    freeToInsert.ValueRef.Pos += blockToRemove.Value.Length;
                }
            }
            else
            {
                currentIdBlock = currentIdBlock.Previous;
            }
        }
        while(currentIdBlock != ids.First);

        return Checksum(ids);
    }

    internal class Block(long id, long pos, long length)
    {
        internal long ID { get; set; } = id;
        internal long Pos { get; set; } = pos;
        internal long Length { get; set; } = length;
    }

    private static (LinkedList<Block> ids, LinkedList<Block> free) SplitLayout(IEnumerable<long> layout)
    {
        var ids  = new LinkedList<Block>();
        var free = new LinkedList<Block>();

        var currentPos      = 0L;
        var isDigit         = true;
        var currentDigit    = 0L;

        foreach(var nextLayout in layout)
        {
            if(isDigit)
            {
                ids.AddLast(new Block(currentDigit, currentPos, nextLayout));
                currentDigit++;
            }
            else
            {
                free.AddLast(new Block(-1, currentPos, nextLayout));
            }

            currentPos += nextLayout;
            isDigit = !isDigit;
        }

        return (ids, free);
    }

    private static void InsertIdBlock(LinkedList<Block> ids, long pos, long id, long length)
    {
        var node = ids.First;
        while(node != null && node.Value.Pos < pos)
        {
            node = node.Next;
        }

        if(node == null)
        {
            ids.AddLast(new Block(id, pos, length));
        }
        else
        {
            ids.AddBefore(node, new Block(id, pos, length));
        }
    }

    private static long Checksum(LinkedList<Block> ids)
        => ids.Sum(idBlock => MathExtensions.SumNumbers(idBlock.Pos, idBlock.Length) * idBlock.ID);
}
