using E2E.Tests.Environment;
using Xunit.Abstractions;
using TaleWorlds.Core;
using HarmonyLib;



namespace E2E.Tests.Services.Equipments;


public class EquipmentCollectionTests : IDisposable
{
    E2ETestEnvironment TestEnvironment { get; }
    public EquipmentCollectionTests(ITestOutputHelper output)
    {
        TestEnvironment = new E2ETestEnvironment(output);
    }

    public void Dispose()
    {
        TestEnvironment.Dispose();
    }

    // TODO create tests similar to Sync BesiegerCamp #977 or Item roster synchronization #701


    [Fact]
    public void ServerUpdateEquipmentCollection_SyncAllClients()
    {
        // Arrange
        var server = TestEnvironment.Server;

        string? EquipmentId = null;


        // Act

        server.Call(() =>
        {
            // No Params
            var Equip = new Equipment();
            Assert.True(server.ObjectManager.TryGetId(Equip, out EquipmentId));


        });

        // Assert
        Assert.True(server.ObjectManager.TryGetObject<Equipment>(EquipmentId, out var Equip));



        foreach (var client in TestEnvironment.Clients)
        {
            Assert.True(client.ObjectManager.TryGetObject<Equipment>(EquipmentId, out var _));


        }

    }


    [Fact]
    public void ClientUpdateEquipmentCollection_DoesNothing()
    {
        // Arrange
        var server = TestEnvironment.Server;
        var client1 = TestEnvironment.Clients.First();


        server.Call(() =>
        {
           
        });

        // Act


        client1.Call(() =>
        {


        });

        // Assert


        foreach (var client in TestEnvironment.Clients)
        {


        }
    }

}