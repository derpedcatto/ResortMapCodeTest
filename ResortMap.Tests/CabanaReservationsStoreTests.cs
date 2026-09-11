using ResortMap.Server.Infrastructure;
using ResortMap.Server.Models;

namespace ResortMap.Tests;

public class CabanaReservationsStoreTests
{
    [Fact]
    public void GetAll_ReturnsEmpty_WhenNothingBooked()
    {
        var store = new CabanaReservationsStore();
        var bookedList = store.GetAll();

        Assert.Empty(bookedList);
    }

    [Fact]
    public void TryAdd_SingleCabana_ReturnsTrueAndAppearsInGetAll()
    {
        var store = new CabanaReservationsStore();
        var cabana = new BookedCabana(new MapCoords(1, 2), new Booking("101", "Alice Smith"));

        var added = store.TryAdd(cabana);

        Assert.True(added);

        var bookedList = store.GetAll();
        Assert.Single(bookedList);
        Assert.Equal(cabana, bookedList[0]);
    }

    [Fact]
    public void TryAdd_UniqueCabanas_SuccessAndAppearInGetAll()
    {
        var store = new CabanaReservationsStore();
        var cabanas = new[]
        {
            new BookedCabana(new MapCoords(0, 0), new Booking("101", "Alice Smith")),
            new BookedCabana(new MapCoords(0, 1), new Booking("102", "Bob Jones")),
            new BookedCabana(new MapCoords(1, 0), new Booking("103", "Carol White")),
        };

        foreach (var cabana in cabanas)
        {
            Assert.True(store.TryAdd(cabana));
        }

        var bookedList = store.GetAll();

        Assert.Equal(cabanas.Length, bookedList.Count);
        Assert.Contains(cabanas[0], bookedList);
        Assert.Contains(cabanas[1], bookedList);
        Assert.Contains(cabanas[2], bookedList);
    }

    [Fact]
    public void TryAdd_TwoReservationsOnSameCoords_FirstSuccessAndSecondReturnsFalse()
    {
        var store = new CabanaReservationsStore();
        var mapCoords = new MapCoords(1, 1);
        var firstReservation = new BookedCabana(mapCoords, new Booking("101", "Alice Smith"));
        var secondReservation = new BookedCabana(mapCoords, new Booking("102", "Bob Jones"));

        Assert.True(store.TryAdd(firstReservation));
        Assert.False(store.TryAdd(secondReservation));

        var bookedList = store.GetAll();
        Assert.Single(bookedList);
        Assert.Equal(firstReservation, bookedList[0]);
    }

    [Fact]
    public void TryAdd_ConcurrentSameCoords_OnlyOneSucceeds()
    {
        var store = new CabanaReservationsStore();
        var mapCoords = new MapCoords(1, 1);
        var attemptCount = 100;
        var successCount = 0;

        Parallel.For(0, attemptCount, i =>
        {
            var booking = new Booking($"Room{i}", $"Guest{i}");
            var cabana = new BookedCabana(mapCoords, booking);
            
            if (store.TryAdd(cabana))
            {
                Interlocked.Increment(ref successCount);
            }
        });

        Assert.Equal(1, successCount);
        Assert.Single(store.GetAll());
    }
}
