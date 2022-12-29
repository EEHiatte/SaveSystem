using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SaveSystem;
using UnityEngine;

[TestFixture]
public class SaveSystemTests
{
    [SetUp]
    public void Setup()
    {
        banana = new Banana
        {
            Id = 420,
            Name = "Banana",
            Value = 69,
            Weight = 50
        };

        booklet = new Item
        {
            Id = 333,
            Name = "Booklet",
            Value = 100
        };

        gun = new Gun
        {
            Id = 777,
            Name = "Gun",
            Value = 1000,
            Damage = 1337,
            MaxAmmo = 3
        };

        itemList = new ItemList
        {
            Items = new List<Item>
            {
                banana,
                booklet,
                gun
            }
        };

        complexObject = new ComplexObject
        {
            Items = itemList,
            PositionObject = new PositionObject
            {
                Position = new Vector3(1, 2, 3)
            },
            RotationObject = new RotationObject
            {
                Rotation = Quaternion.Euler(3, 2, 1)
            },
            ScaleObject = new ScaleObject
            {
                Scale = new Vector3(3, 2, 1)
            }
        };
    }

    private Banana banana;
    private Item booklet;
    private Gun gun;
    private ItemList itemList;
    private ComplexObject complexObject;

    [Test]
    public void JsonSerializationTest()
    {
        var bananaJson = SaveSystem.SaveSystem.SerializeToJson(banana);
        var bookletJson = SaveSystem.SaveSystem.SerializeToJson(booklet);
        var gunJson = SaveSystem.SaveSystem.SerializeToJson(gun);
        var itemListJson = SaveSystem.SaveSystem.SerializeToJson(itemList);
        var complexObjectJson = SaveSystem.SaveSystem.SerializeToJson(complexObject);
        
        Debug.Log(bananaJson);
        Debug.Log(bookletJson);
        Debug.Log(gunJson);
        Debug.Log(itemListJson);
        Debug.Log(complexObjectJson);

        var newBanana = SaveSystem.SaveSystem.DeserializeFromJson<Banana>(bananaJson);
        var newBooklet = SaveSystem.SaveSystem.DeserializeFromJson<Item>(bookletJson);
        var newGun = SaveSystem.SaveSystem.DeserializeFromJson<Gun>(gunJson);
        var newItemList = SaveSystem.SaveSystem.DeserializeFromJson<ItemList>(itemListJson);
        var newComplexObject = SaveSystem.SaveSystem.DeserializeFromJson<ComplexObject>(complexObjectJson);

        Assert.NotNull(newBanana);
        Assert.NotNull(newBooklet);
        Assert.NotNull(newGun);
        Assert.NotNull(newItemList);
        Assert.NotNull(newComplexObject);

        Assert.AreEqual(banana.Id, newBanana.Id);
        Assert.AreEqual(banana.Name, newBanana.Name);
        Assert.AreEqual(banana.Value, newBanana.Value);
        Assert.AreEqual(banana.Weight, newBanana.Weight);

        Assert.AreEqual(booklet.Id, newBooklet.Id);
        Assert.AreEqual(booklet.Name, newBooklet.Name);
        Assert.AreEqual(booklet.Value, newBooklet.Value);

        Assert.AreEqual(gun.Id, newGun.Id);
        Assert.AreEqual(gun.Name, newGun.Name);
        Assert.AreEqual(gun.Value, newGun.Value);
        Assert.AreEqual(gun.Damage, newGun.Damage);
        Assert.AreEqual(gun.MaxAmmo, newGun.MaxAmmo);

        Assert.NotNull(newItemList.Items);
        Assert.IsNotEmpty(newItemList.Items);
        Assert.AreEqual(itemList.Items.Count, newItemList.Items.Count);
        for (var i = 0; i < itemList.Items.Count; i++)
        {
            Assert.IsTrue(itemList.Items[i].Equals(newItemList.Items[i]));
        }

        Assert.NotNull(newComplexObject.Items);
        Assert.IsNotEmpty(newComplexObject.Items.Items);
        Assert.AreEqual(complexObject.Items.Items.Count, newComplexObject.Items.Items.Count);
        for (var i = 0; i < complexObject.Items.Items.Count; i++)
            Assert.IsTrue(complexObject.Items.Items[i].Equals(newComplexObject.Items.Items[i]));
        Assert.NotNull(newComplexObject.PositionObject);
        Assert.IsTrue(complexObject.PositionObject.Position == (newComplexObject.PositionObject.Position));
        Assert.NotNull(newComplexObject.RotationObject);
        Assert.IsTrue(complexObject.RotationObject.Rotation == (newComplexObject.RotationObject.Rotation));
        Assert.NotNull(newComplexObject.ScaleObject);
        Assert.IsTrue(complexObject.ScaleObject.Scale == (newComplexObject.ScaleObject.Scale));
    }

    [Test]
    public void BinarySerializationTest()
    {
        var bananaBin = SaveSystem.SaveSystem.SerializeToBinary(banana);
        var bookletBin = SaveSystem.SaveSystem.SerializeToBinary(booklet);
        var gunBin = SaveSystem.SaveSystem.SerializeToBinary(gun);
        var itemListBin = SaveSystem.SaveSystem.SerializeToBinary(itemList);
        var complexObjectBin = SaveSystem.SaveSystem.SerializeToBinary(complexObject);
        
        var newBanana = SaveSystem.SaveSystem.DeserializeFromBinary<Banana>(bananaBin);
        var newBooklet = SaveSystem.SaveSystem.DeserializeFromBinary<Item>(bookletBin);
        var newGun = SaveSystem.SaveSystem.DeserializeFromBinary<Gun>(gunBin);
        var newItemList = SaveSystem.SaveSystem.DeserializeFromBinary<ItemList>(itemListBin);
        var newComplexObject = SaveSystem.SaveSystem.DeserializeFromBinary<ComplexObject>(complexObjectBin);

        Assert.NotNull(newBanana);
        Assert.NotNull(newBooklet);
        Assert.NotNull(newGun);
        Assert.NotNull(newItemList);
        Assert.NotNull(newComplexObject);

        Assert.AreEqual(banana.Id, newBanana.Id);
        Assert.AreEqual(banana.Name, newBanana.Name);
        Assert.AreEqual(banana.Value, newBanana.Value);
        Assert.AreEqual(banana.Weight, newBanana.Weight);

        Assert.AreEqual(booklet.Id, newBooklet.Id);
        Assert.AreEqual(booklet.Name, newBooklet.Name);
        Assert.AreEqual(booklet.Value, newBooklet.Value);

        Assert.AreEqual(gun.Id, newGun.Id);
        Assert.AreEqual(gun.Name, newGun.Name);
        Assert.AreEqual(gun.Value, newGun.Value);
        Assert.AreEqual(gun.Damage, newGun.Damage);
        Assert.AreEqual(gun.MaxAmmo, newGun.MaxAmmo);

        Assert.NotNull(newItemList.Items);
        Assert.IsNotEmpty(newItemList.Items);
        Assert.AreEqual(itemList.Items.Count, newItemList.Items.Count);
        for (var i = 0; i < itemList.Items.Count; i++)
        {
            Assert.IsTrue(itemList.Items[i].Equals(newItemList.Items[i]));
        }

        Assert.NotNull(newComplexObject.Items);
        Assert.IsNotEmpty(newComplexObject.Items.Items);
        Assert.AreEqual(complexObject.Items.Items.Count, newComplexObject.Items.Items.Count);
        for (var i = 0; i < complexObject.Items.Items.Count; i++)
            Assert.IsTrue(complexObject.Items.Items[i].Equals(newComplexObject.Items.Items[i]));
        Assert.NotNull(newComplexObject.PositionObject);
        Assert.IsTrue(complexObject.PositionObject.Position == (newComplexObject.PositionObject.Position));
        Assert.NotNull(newComplexObject.RotationObject);
        Assert.IsTrue(complexObject.RotationObject.Rotation == (newComplexObject.RotationObject.Rotation));
        Assert.NotNull(newComplexObject.ScaleObject);
        Assert.IsTrue(complexObject.ScaleObject.Scale == (newComplexObject.ScaleObject.Scale));
    }

    [Test]
    public void JsonFileSerializationTest()
    {
        Debug.Log("Starting JsonFileSerializationTest");
        var settings = new SaveSystemSettings() { format = SaveSystem.SaveSystem.Format.JSON };
        settings.location = SaveSystem.SaveSystem.Location.PersistentDataPath;
        FileTest("TestData/JsonFileSerializationTest.tst", settings);
        settings.location = SaveSystem.SaveSystem.Location.StreamingAssetsPath;
        FileTest("TestData/JsonFileSerializationTest.tst", settings);
        settings.location = SaveSystem.SaveSystem.Location.PlayerPrefs;
        FileTest("TestData/JsonFileSerializationTest.tst", settings);
        Debug.Log("JsonFileSerializationTest Complete");
    }

    [Test]
    public void BinaryFileSerializationTest()
    {
        Debug.Log("Starting BinaryFileSerializationTest");
        var settings = new SaveSystemSettings() { format = SaveSystem.SaveSystem.Format.Binary };
        settings.location = SaveSystem.SaveSystem.Location.PersistentDataPath;
        FileTest("TestData/JsonFileSerializationTest.tst", settings);
        settings.location = SaveSystem.SaveSystem.Location.StreamingAssetsPath;
        FileTest("TestData/JsonFileSerializationTest.tst", settings);
        settings.location = SaveSystem.SaveSystem.Location.PlayerPrefs;
        FileTest("TestData/JsonFileSerializationTest.tst", settings);
        Debug.Log("BinaryFileSerializationTest Complete");
    }
    
    [Test]
    public void JsonCompressedFileSerializationTest()
    {
        Debug.Log("Starting JsonCompressedFileSerializationTest");
        var settings = new SaveSystemSettings() { format = SaveSystem.SaveSystem.Format.JSON, compress = true };
        settings.location = SaveSystem.SaveSystem.Location.PersistentDataPath;
        FileTest("TestData/JsonFileSerializationTest.tst", settings);
        settings.location = SaveSystem.SaveSystem.Location.StreamingAssetsPath;
        FileTest("TestData/JsonFileSerializationTest.tst", settings);
        settings.location = SaveSystem.SaveSystem.Location.PlayerPrefs;
        FileTest("TestData/JsonFileSerializationTest.tst", settings);
        Debug.Log("JsonCompressedFileSerializationTest Complete");
    }

    [Test]
    public void BinaryCompressedFileSerializationTest()
    {
        Debug.Log("Starting BinaryCompressedFileSerializationTest");
        var settings = new SaveSystemSettings() { format = SaveSystem.SaveSystem.Format.Binary,  compress = true };
        settings.location = SaveSystem.SaveSystem.Location.PersistentDataPath;
        FileTest("TestData/JsonFileSerializationTest.tst", settings);
        settings.location = SaveSystem.SaveSystem.Location.StreamingAssetsPath;
        FileTest("TestData/JsonFileSerializationTest.tst", settings);
        settings.location = SaveSystem.SaveSystem.Location.PlayerPrefs;
        FileTest("TestData/JsonFileSerializationTest.tst", settings);
        Debug.Log("BinaryCompressedFileSerializationTest Complete");
    }

    [Test]
    public void CompressedJsonSerializationTest()
    {
        Debug.Log("Starting CompressedJsonFileSerializationTest");
        var bananaJson = SaveSystem.SaveSystem.SerializeToJson(banana);
        var bookletJson = SaveSystem.SaveSystem.SerializeToJson(booklet);
        var gunJson = SaveSystem.SaveSystem.SerializeToJson(gun);
        var itemListJson = SaveSystem.SaveSystem.SerializeToJson(itemList);
        var complexObjectJson = SaveSystem.SaveSystem.SerializeToJson(complexObject);
        
        var bananaCompJson = SaveSystem.SaveSystem.Compression.Compress(Encoding.UTF8.GetBytes(bananaJson));
        var bookletCompJson = SaveSystem.SaveSystem.Compression.Compress(Encoding.UTF8.GetBytes(bookletJson));
        var gunCompJson = SaveSystem.SaveSystem.Compression.Compress(Encoding.UTF8.GetBytes(gunJson));
        var itemListCompJson = SaveSystem.SaveSystem.Compression.Compress(Encoding.UTF8.GetBytes(itemListJson));
        var complexObjectCompJson = SaveSystem.SaveSystem.Compression.Compress(Encoding.UTF8.GetBytes(complexObjectJson));
        
        Debug.Log($"Compressing: {DebugByteArray(Encoding.UTF8.GetBytes(bananaJson))}");
        Debug.Log($"Compressed: {DebugByteArray(bananaCompJson)}");
        Debug.Log($"Compressing: {DebugByteArray(Encoding.UTF8.GetBytes(bookletJson))}");
        Debug.Log($"Compressed: {DebugByteArray(bookletCompJson)}");
        Debug.Log($"Compressing: {DebugByteArray(Encoding.UTF8.GetBytes(gunJson))}");
        Debug.Log($"Compressed: {DebugByteArray(gunCompJson)}");
        Debug.Log($"Compressing: {DebugByteArray(Encoding.UTF8.GetBytes(itemListJson))}");
        Debug.Log($"Compressed: {DebugByteArray(itemListCompJson)}");
        Debug.Log($"Compressing: {DebugByteArray(Encoding.UTF8.GetBytes(complexObjectJson))}");
        Debug.Log($"Compressed: {DebugByteArray(complexObjectCompJson)}");
        
        Assert.IsTrue(SaveSystem.SaveSystem.Compression.IsCompressed(bananaCompJson));
        Assert.IsTrue(SaveSystem.SaveSystem.Compression.IsCompressed(bookletCompJson));
        Assert.IsTrue(SaveSystem.SaveSystem.Compression.IsCompressed(gunCompJson));
        Assert.IsTrue(SaveSystem.SaveSystem.Compression.IsCompressed(itemListCompJson));
        Assert.IsTrue(SaveSystem.SaveSystem.Compression.IsCompressed(complexObjectCompJson));
        
        bananaJson = Encoding.UTF8.GetString(SaveSystem.SaveSystem.Compression.Decompress(bananaCompJson));
        bookletJson = Encoding.UTF8.GetString(SaveSystem.SaveSystem.Compression.Decompress(bookletCompJson));
        gunJson = Encoding.UTF8.GetString(SaveSystem.SaveSystem.Compression.Decompress(gunCompJson));
        itemListJson = Encoding.UTF8.GetString(SaveSystem.SaveSystem.Compression.Decompress(itemListCompJson));
        complexObjectJson = Encoding.UTF8.GetString(SaveSystem.SaveSystem.Compression.Decompress(complexObjectCompJson));
        
        Debug.Log(bananaJson);
        Debug.Log(bookletJson);
        Debug.Log(gunJson);
        Debug.Log(itemListJson);
        Debug.Log(complexObjectJson);

        var newBanana = SaveSystem.SaveSystem.DeserializeFromJson<Banana>(bananaJson);
        var newBooklet = SaveSystem.SaveSystem.DeserializeFromJson<Item>(bookletJson);
        var newGun = SaveSystem.SaveSystem.DeserializeFromJson<Gun>(gunJson);
        var newItemList = SaveSystem.SaveSystem.DeserializeFromJson<ItemList>(itemListJson);
        var newComplexObject = SaveSystem.SaveSystem.DeserializeFromJson<ComplexObject>(complexObjectJson);

        Assert.NotNull(newBanana);
        Assert.NotNull(newBooklet);
        Assert.NotNull(newGun);
        Assert.NotNull(newItemList);
        Assert.NotNull(newComplexObject);

        Assert.AreEqual(banana.Id, newBanana.Id);
        Assert.AreEqual(banana.Name, newBanana.Name);
        Assert.AreEqual(banana.Value, newBanana.Value);
        Assert.AreEqual(banana.Weight, newBanana.Weight);

        Assert.AreEqual(booklet.Id, newBooklet.Id);
        Assert.AreEqual(booklet.Name, newBooklet.Name);
        Assert.AreEqual(booklet.Value, newBooklet.Value);

        Assert.AreEqual(gun.Id, newGun.Id);
        Assert.AreEqual(gun.Name, newGun.Name);
        Assert.AreEqual(gun.Value, newGun.Value);
        Assert.AreEqual(gun.Damage, newGun.Damage);
        Assert.AreEqual(gun.MaxAmmo, newGun.MaxAmmo);

        Assert.NotNull(newItemList.Items);
        Assert.IsNotEmpty(newItemList.Items);
        Assert.AreEqual(itemList.Items.Count, newItemList.Items.Count);
        for (var i = 0; i < itemList.Items.Count; i++)
        {
            Assert.IsTrue(itemList.Items[i].Equals(newItemList.Items[i]));
        }

        Assert.NotNull(newComplexObject.Items);
        Assert.IsNotEmpty(newComplexObject.Items.Items);
        Assert.AreEqual(complexObject.Items.Items.Count, newComplexObject.Items.Items.Count);
        for (var i = 0; i < complexObject.Items.Items.Count; i++)
            Assert.IsTrue(complexObject.Items.Items[i].Equals(newComplexObject.Items.Items[i]));
        Assert.NotNull(newComplexObject.PositionObject);
        Assert.IsTrue(complexObject.PositionObject.Position == (newComplexObject.PositionObject.Position));
        Assert.NotNull(newComplexObject.RotationObject);
        Assert.IsTrue(complexObject.RotationObject.Rotation == (newComplexObject.RotationObject.Rotation));
        Assert.NotNull(newComplexObject.ScaleObject);
        Assert.IsTrue(complexObject.ScaleObject.Scale == (newComplexObject.ScaleObject.Scale));
        Debug.Log("CompressedJsonFileSerializationTest Complete");
    }

    [Test]
    public void CompressedBinarySerializationTest()
    {
        var bananaBin = SaveSystem.SaveSystem.SerializeToBinary(banana);
        var bookletBin = SaveSystem.SaveSystem.SerializeToBinary(booklet);
        var gunBin = SaveSystem.SaveSystem.SerializeToBinary(gun);
        var itemListBin = SaveSystem.SaveSystem.SerializeToBinary(itemList);
        var complexObjectBin = SaveSystem.SaveSystem.SerializeToBinary(complexObject);
        
        var bananaComp = SaveSystem.SaveSystem.Compression.Compress(bananaBin);
        var bookletComp = SaveSystem.SaveSystem.Compression.Compress(bookletBin);
        var gunComp = SaveSystem.SaveSystem.Compression.Compress(gunBin);
        var itemListComp = SaveSystem.SaveSystem.Compression.Compress(itemListBin);
        var complexObjectComp = SaveSystem.SaveSystem.Compression.Compress(complexObjectBin);

        Assert.IsTrue(SaveSystem.SaveSystem.Compression.IsCompressed(bananaComp));
        Assert.IsTrue(SaveSystem.SaveSystem.Compression.IsCompressed(bookletComp));
        Assert.IsTrue(SaveSystem.SaveSystem.Compression.IsCompressed(gunComp));
        Assert.IsTrue(SaveSystem.SaveSystem.Compression.IsCompressed(itemListComp));
        Assert.IsTrue(SaveSystem.SaveSystem.Compression.IsCompressed(complexObjectComp));
        
        bananaBin = (SaveSystem.SaveSystem.Compression.Decompress(bananaComp));
        bookletBin = (SaveSystem.SaveSystem.Compression.Decompress(bookletComp));
        gunBin = (SaveSystem.SaveSystem.Compression.Decompress(gunComp));
        itemListBin = (SaveSystem.SaveSystem.Compression.Decompress(itemListComp));
        complexObjectBin = (SaveSystem.SaveSystem.Compression.Decompress(complexObjectComp));
 
        var newBanana = SaveSystem.SaveSystem.DeserializeFromBinary<Banana>(bananaBin);
        var newBooklet = SaveSystem.SaveSystem.DeserializeFromBinary<Item>(bookletBin);
        var newGun = SaveSystem.SaveSystem.DeserializeFromBinary<Gun>(gunBin);
        var newItemList = SaveSystem.SaveSystem.DeserializeFromBinary<ItemList>(itemListBin);
        var newComplexObject = SaveSystem.SaveSystem.DeserializeFromBinary<ComplexObject>(complexObjectBin);

        Assert.NotNull(newBanana);
        Assert.NotNull(newBooklet);
        Assert.NotNull(newGun);
        Assert.NotNull(newItemList);
        Assert.NotNull(newComplexObject);

        Assert.AreEqual(banana.Id, newBanana.Id);
        Assert.AreEqual(banana.Name, newBanana.Name);
        Assert.AreEqual(banana.Value, newBanana.Value);
        Assert.AreEqual(banana.Weight, newBanana.Weight);

        Assert.AreEqual(booklet.Id, newBooklet.Id);
        Assert.AreEqual(booklet.Name, newBooklet.Name);
        Assert.AreEqual(booklet.Value, newBooklet.Value);

        Assert.AreEqual(gun.Id, newGun.Id);
        Assert.AreEqual(gun.Name, newGun.Name);
        Assert.AreEqual(gun.Value, newGun.Value);
        Assert.AreEqual(gun.Damage, newGun.Damage);
        Assert.AreEqual(gun.MaxAmmo, newGun.MaxAmmo);

        Assert.NotNull(newItemList.Items);
        Assert.IsNotEmpty(newItemList.Items);
        Assert.AreEqual(itemList.Items.Count, newItemList.Items.Count);
        for (var i = 0; i < itemList.Items.Count; i++)
        {
            Assert.IsTrue(itemList.Items[i].Equals(newItemList.Items[i]));
        }

        Assert.NotNull(newComplexObject.Items);
        Assert.IsNotEmpty(newComplexObject.Items.Items);
        Assert.AreEqual(complexObject.Items.Items.Count, newComplexObject.Items.Items.Count);
        for (var i = 0; i < complexObject.Items.Items.Count; i++)
            Assert.IsTrue(complexObject.Items.Items[i].Equals(newComplexObject.Items.Items[i]));
        Assert.NotNull(newComplexObject.PositionObject);
        Assert.IsTrue(complexObject.PositionObject.Position == (newComplexObject.PositionObject.Position));
        Assert.NotNull(newComplexObject.RotationObject);
        Assert.IsTrue(complexObject.RotationObject.Rotation == (newComplexObject.RotationObject.Rotation));
        Assert.NotNull(newComplexObject.ScaleObject);
        Assert.IsTrue(complexObject.ScaleObject.Scale == (newComplexObject.ScaleObject.Scale));
    }
    
    private void FileTest(string path, SaveSystemSettings settings)
    {
        Debug.Log($"Save Begin\nPath: '{path}' Location: '{settings.location}' Format: '{settings.format}' Compressed: '{settings.compress}'");
        SaveSystem.SaveSystem.Save("BananaKey", banana, path, settings);
        SaveSystem.SaveSystem.Save("BookletKey", booklet, path, settings);
        SaveSystem.SaveSystem.Save("GunKey", gun, path, settings);
        SaveSystem.SaveSystem.Save("ItemListKey", itemList, path, settings);
        SaveSystem.SaveSystem.Save("ComplexObjectKey", complexObject, path, settings);
        Debug.Log("Save Complete");
        Debug.Log($"Load Begin\nPath: '{path}' Location: '{settings.location} Format: '{settings.format}' Compressed: '{settings.compress}'");

        var newBanana = SaveSystem.SaveSystem.Load<Banana>("BananaKey", path, settings.location);
        var newBooklet = SaveSystem.SaveSystem.Load<Item>("BookletKey", path, settings.location);
        var newGun = SaveSystem.SaveSystem.Load<Gun>("GunKey", path, settings.location);
        var newItemList = SaveSystem.SaveSystem.Load<ItemList>("ItemListKey", path, settings.location);
        var newComplexObject = SaveSystem.SaveSystem.Load<ComplexObject>("ComplexObjectKey", path, settings.location);
        Debug.Log("Load Complete");
        SaveSystem.SaveSystem.Delete(path, settings.location);
        
        Assert.NotNull(newBanana);
        Assert.NotNull(newBooklet);
        Assert.NotNull(newGun);
        Assert.NotNull(newItemList);
        Assert.NotNull(newComplexObject);

        Assert.AreEqual(banana.Id, newBanana.Id);
        Assert.AreEqual(banana.Name, newBanana.Name);
        Assert.AreEqual(banana.Value, newBanana.Value);
        Assert.AreEqual(banana.Weight, newBanana.Weight);

        Assert.AreEqual(booklet.Id, newBooklet.Id);
        Assert.AreEqual(booklet.Name, newBooklet.Name);
        Assert.AreEqual(booklet.Value, newBooklet.Value);

        Assert.AreEqual(gun.Id, newGun.Id);
        Assert.AreEqual(gun.Name, newGun.Name);
        Assert.AreEqual(gun.Value, newGun.Value);
        Assert.AreEqual(gun.Damage, newGun.Damage);
        Assert.AreEqual(gun.MaxAmmo, newGun.MaxAmmo);

        Assert.NotNull(newItemList.Items);
        Assert.IsNotEmpty(newItemList.Items);
        Assert.AreEqual(itemList.Items.Count, newItemList.Items.Count);
        for (var i = 0; i < itemList.Items.Count; i++)
        {
            Assert.IsTrue(itemList.Items[i].Equals(newItemList.Items[i]));
        }

        Assert.NotNull(newComplexObject.Items);
        Assert.IsNotEmpty(newComplexObject.Items.Items);
        Assert.AreEqual(complexObject.Items.Items.Count, newComplexObject.Items.Items.Count);
        for (var i = 0; i < complexObject.Items.Items.Count; i++)
            Assert.IsTrue(complexObject.Items.Items[i].Equals(newComplexObject.Items.Items[i]));
        Assert.NotNull(newComplexObject.PositionObject);
        Assert.IsTrue(complexObject.PositionObject.Position == (newComplexObject.PositionObject.Position));
        Assert.NotNull(newComplexObject.RotationObject);
        Assert.IsTrue(complexObject.RotationObject.Rotation == (newComplexObject.RotationObject.Rotation));
        Assert.NotNull(newComplexObject.ScaleObject);
        Assert.IsTrue(complexObject.ScaleObject.Scale == (newComplexObject.ScaleObject.Scale));
    }

    private string DebugByteArray(byte[] data)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendJoin(',', data);
        return stringBuilder.ToString();
    }

    [Serializable]
    private class Item
    {
        [SerializeField] private int id;

        [SerializeField] private string name;

        [SerializeField] private int value;

        public int Id
        {
            get => id;
            set => id = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public int Value
        {
            get => value;
            set => this.value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj is Item item) return item.Id == Id && item.Name == Name && item.Value == Value;

            return false;
        }
    }

    [Serializable]
    private class Banana : Item
    {
        [SerializeField] private int weight;

        public int Weight
        {
            get => weight;
            set => weight = value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj is Banana banana)
                return banana.Id == Id && banana.Name == Name && banana.Value == Value && banana.Weight == Weight;

            return false;
        }
    }

    [Serializable]
    private class Gun : Item
    {
        [SerializeField] private int damage;

        [SerializeField] private int maxAmmo;

        public int Damage
        {
            get => damage;
            set => damage = value;
        }

        public int MaxAmmo
        {
            get => maxAmmo;
            set => maxAmmo = value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj is Gun gun)
                return gun.Id == Id && gun.Name == Name && gun.Value == Value && gun.Damage == Damage &&
                       gun.MaxAmmo == MaxAmmo;

            return false;
        }
    }

    private class ComplexObject
    {
        [SerializeField] private ItemList items;
        [SerializeField] private PositionObject positionObject;
        [SerializeField] private RotationObject rotationObject;
        [SerializeField] private ScaleObject scaleObject;

        public ItemList Items
        {
            get => items;
            set => items = value;
        }

        public PositionObject PositionObject
        {
            get => positionObject;
            set => positionObject = value;
        }

        public RotationObject RotationObject
        {
            get => rotationObject;
            set => rotationObject = value;
        }

        public ScaleObject ScaleObject
        {
            get => scaleObject;
            set => scaleObject = value;
        }
    }

    [Serializable]
    private class PositionObject
    {
        [SerializeField] private Vector3 position;

        public Vector3 Position
        {
            get => position;
            set => position = value;
        }
    }

    [Serializable]
    private class RotationObject
    {
        [SerializeField] private Quaternion rotation;

        public Quaternion Rotation
        {
            get => rotation;
            set => rotation = value;
        }
    }

    [Serializable]
    private class ScaleObject
    {
        [SerializeField] private Vector3 scale;

        public Vector3 Scale
        {
            get => scale;
            set => scale = value;
        }
    }

    [Serializable]
    private class ItemList
    {
        [SerializeField] private List<Item> items;

        public List<Item> Items
        {
            get => items;
            set => items = value;
        }
    }
}