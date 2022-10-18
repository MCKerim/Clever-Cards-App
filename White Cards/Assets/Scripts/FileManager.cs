using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public static class FileManager
{
    public static void SaveCategories(List<Category> categories){
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/categories.MCKerimData";
        FileStream stream = new FileStream(path, FileMode.Create);

        CategoriesData categoriesData = new CategoriesData(categories);

        formatter.Serialize(stream, categoriesData);
        stream.Close();
        Debug.Log("Categories Saved in: " + path);
    }

    public static void SaveCardsOfCategory(Category category, List<Card> cards){
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/" + category.Uuid + ".MCKerimData";
        FileStream stream = new FileStream(path, FileMode.Create);

        CardsFromCategoryData cardsFromCategoryData = new CardsFromCategoryData(cards);

        formatter.Serialize(stream, cardsFromCategoryData);
        stream.Close();
        Debug.Log("Cards Saved in: " + path);
    }

    public static List<Category> LoadCategoriesFromFile()
    {
        string path = Application.persistentDataPath + "/categories.MCKerimData";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            CategoriesData categoriesData = formatter.Deserialize(stream) as CategoriesData;

            stream.Close();
            Debug.Log("Save File Found in: " + path);
            return categoriesData.categories;
        }
        else
        {
            Debug.Log("Save File not found in " + path);
            List<Category> categories = new List<Category>();
            return categories;
        }
    }

    public static List<Card> LoadCardsOfCategoryFromFile(Category category)
    {
        string path = Application.persistentDataPath + "/" + category.Uuid + ".MCKerimData";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            CardsFromCategoryData cardsFromCategoryData = formatter.Deserialize(stream) as CardsFromCategoryData;

            stream.Close();
            Debug.Log("Save File Found in: " + path);
            return cardsFromCategoryData.cards;
        }
        else
        {
            Debug.Log("Save File not found in " + path);
            List<Card> cards = new List<Card>();
            return cards;
        }
    }

    public static void ShareCategory(Category category)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/" + category.Name + ".txt";
        FileStream stream = new FileStream(path, FileMode.Create);

        ShareableCategoryData shareableCategoryData = new ShareableCategoryData(category, LoadCardsOfCategoryFromFile(category));

        formatter.Serialize(stream, shareableCategoryData);
        stream.Close();

        new NativeShare().AddFile( path )
		.SetSubject( category.Name + " Category" ).SetText( "Import this file in the White Cards App to start learning!" ).SetUrl( "https://MCKerim.com" )
		.SetCallback( ( result, shareTarget ) => 
        {
            Debug.Log("Share result: " + result + ", selected app: " + shareTarget);
            File.Delete(path); 
        })
		.Share();
    }
}
