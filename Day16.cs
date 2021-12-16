namespace aoc_2021;

public class Day16
{
    public static class BITS
    {
        public static IEnumerable<bool> HexToBits(string input)
        {
            for (var i = 0; i < input.Length; i++)
            {
                var hex = input.AsSpan(i, 1);
                var dec = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                for (var j = 0b1000; j > 0; j = j >> 1)
                {
                    yield return (dec & j) == j;
                }
            }
        }

        public static bool TryReadNumber(IEnumerator<bool> bitStream, long numberOfBits, out long value)
        {
            value = default;
            var bitsRead = ReadBits(bitStream, numberOfBits).ToList();
            if (bitsRead.Count != numberOfBits) return false;

            using var bitsReadStream = bitsRead.GetEnumerator();
            value = ReadNumber(bitsReadStream, numberOfBits);
            return true;
        }
        public static long ReadNumber(IEnumerator<bool> bitStream, long numberOfBits) =>
            ReadBits(bitStream, numberOfBits)
                .Select((bit, index) => bit ? 1L << (int)(numberOfBits - index - 1) : 0L)
                .Sum();

        public static IEnumerable<bool> ReadBits(IEnumerator<bool> bitStream, long numberOfBits)
        {
            while (numberOfBits-- > 0 && bitStream.MoveNext())
            {
                yield return bitStream.Current;
            }
        }
        public interface IPacket
        {
            long Version { get; }
            long TypeId { get; }
        }
        public record LiteralValuePacket(long Version, long TypeId, long Value) : IPacket;
        public record OperatorPacket(long Version, long TypeId, long LengthTypeId, long Length, ImmutableList<IPacket> SubPackets) : IPacket;

        public static bool TryReadPacket(IEnumerator<bool> bitStream, out IPacket packet)
        {
            packet = default;
            if (!TryReadNumber(bitStream, 3, out var version) || !TryReadNumber(bitStream, 3, out var typeId))
            {
                return false;
            }

            if (typeId == 4)
            {
                var literalValueBits = new List<bool>();
                var lastGroup = false;
                do
                {
                    lastGroup = ReadNumber(bitStream, 1) == 0;
                    literalValueBits.AddRange(ReadBits(bitStream, 4));
                } while (!lastGroup);
                using var literalValueBitStream = literalValueBits.GetEnumerator();
                var value = ReadNumber(literalValueBitStream, literalValueBits.Count);
                packet = new LiteralValuePacket(version, typeId, value);
                return true;
            }

            var lengthTypeId = ReadNumber(bitStream, 1);
            var length = ReadNumber(bitStream, lengthTypeId == 0 ? 15 : 11);
            var subPackets = new List<IPacket>();

            if (lengthTypeId == 0)
            {
                using var subPacketStream = ReadBits(bitStream, length).GetEnumerator();
                while (TryReadPacket(subPacketStream, out var subPacket))
                    subPackets.Add(subPacket);
                
            }
            else
            {
                for (var i = 0; i < length; i++)
                {
                    if (TryReadPacket(bitStream, out var subPacket))
                    {
                        subPackets.Add(subPacket);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            packet = new OperatorPacket(version, typeId, lengthTypeId, length, subPackets.ToImmutableList());
            return true;
        }
    }


    [Fact]
    public void HexToBits()
    {
        var bits = BITS.HexToBits("D2FE28").ToArray();
        var result = string.Join("", bits.Select(b => b ? '1' : '0'));
        Assert.Equal("110100101111111000101000", result);
    }

    [Fact]
    public void ReadNumber()
    {
        using var bitStream = "11010010".Select(c => c == '1').GetEnumerator();
        Assert.Equal(6, BITS.ReadNumber(bitStream, 3));
        Assert.Equal(4, BITS.ReadNumber(bitStream, 3));
        Assert.Equal(1, BITS.ReadNumber(bitStream, 1));
        Assert.Equal(0, BITS.ReadNumber(bitStream, 1));
    }

    [Fact]
    public void ReadValuePacket()
    {
        using var bitStream = "110100101111111000101000".Select(c => c == '1').GetEnumerator();
        Assert.True(BITS.TryReadPacket(bitStream, out var packet));
        Assert.IsType<BITS.LiteralValuePacket>(packet);
        Assert.Equal(2021, packet is BITS.LiteralValuePacket lvp ? lvp.Value : -1);
        Assert.False(BITS.TryReadPacket(bitStream, out _));
    }


    [Theory]
    [InlineData("38006F45291200", 2)]
    [InlineData("EE00D40C823060", 3)]
    public void ReadOperatorPacket(string input, int expectedSubPackets)
    {
        using var bitStream = BITS.HexToBits(input).GetEnumerator();

        Assert.True(BITS.TryReadPacket(bitStream, out var packet));
        Assert.IsType<BITS.OperatorPacket>(packet);
        Assert.Equal(expectedSubPackets, packet is BITS.OperatorPacket op ? op.SubPackets.Count : -1);
    }

    [Theory]
    [InlineData("8A004A801A8002F478", 16)]
    [InlineData("620080001611562C8802118E34", 12)]
    [InlineData("C0015000016115A2E0802F182340", 23)]
    [InlineData("A0016C880162017C3686B18A3D4780", 31)]
    [InlineData(null, 895)]
    public void Part1(string input, int expectedResult)
    {
        static long SumVersions(IEnumerable<BITS.IPacket> packets) =>
            packets.Sum(p => p.Version + (p is BITS.OperatorPacket op ? SumVersions(op.SubPackets) : 0));

        input = input ?? LoadData("Day16_Input");
        using var bitStream = BITS.HexToBits(input).GetEnumerator();
        Assert.True(BITS.TryReadPacket(bitStream, out var packet));
        var result = SumVersions(new [] { packet });

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(null, 1148595959144)]
    public void Part2(string input, long expectedResult)
    {
        static long Evaluate(BITS.IPacket packet)
            => packet switch
            {
                BITS.LiteralValuePacket lvp => lvp.Value, 
                BITS.OperatorPacket op when op.TypeId == 0 => op.SubPackets.Sum(Evaluate),
                BITS.OperatorPacket op when op.TypeId == 1 => op.SubPackets.Aggregate(1L, (acc, cur) => acc * Evaluate(cur)),
                BITS.OperatorPacket op when op.TypeId == 2 => op.SubPackets.Min(Evaluate),
                BITS.OperatorPacket op when op.TypeId == 3 => op.SubPackets.Max(Evaluate),
                BITS.OperatorPacket op when op.TypeId == 5 => Evaluate(op.SubPackets[0]) > Evaluate(op.SubPackets[1]) ? 1L : 0L,
                BITS.OperatorPacket op when op.TypeId == 6 => Evaluate(op.SubPackets[0]) < Evaluate(op.SubPackets[1]) ? 1L : 0L,
                BITS.OperatorPacket op when op.TypeId == 7 => Evaluate(op.SubPackets[0]) == Evaluate(op.SubPackets[1]) ? 1L : 0L,
                _ => throw new InvalidDataException($"unexpected packet type {packet.TypeId}"),
            };

        input = input ?? LoadData("Day16_Input");
        using var bitStream = BITS.HexToBits(input).GetEnumerator();
        Assert.True(BITS.TryReadPacket(bitStream, out var packet));

        var result = Evaluate(packet);

        Assert.Equal(expectedResult, result);
    }
}