﻿using MauiPokemon.Models;
using SQLite;
using System.Reflection;

namespace MauiPokemon.Services
{
    public class PokemonDatabaseService
    {
        private SQLiteConnection _dbConnection;

        public string GetDatabasePath()
        {
            string filename = "pokemon.db";
            string pathToDb = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(pathToDb, filename);
        }

        private void ExtractDbEmbeddedResource()
        {
            var assembly = typeof(PokemonDatabaseService).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("MauiPokemon.EmbeddedDb.pokemon.db");

            var path = GetDatabasePath();

            if (stream != null)
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        BinaryWriter bw = new BinaryWriter(fs);
                        byte[] bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                        bw.Write(bytes);
                    }
                }
            }
        }

        public PokemonDatabaseService()
        {
            if (!File.Exists(GetDatabasePath()))
                ExtractDbEmbeddedResource();

            _dbConnection = new SQLiteConnection(GetDatabasePath());
        }

        public List<Pokemon> GetAllPokemon()
        {
            return _dbConnection.Table<Pokemon>().ToList();
        }

        public Ability GetAbility(int id)
        {
            return _dbConnection.Table<Ability>().Where(x => x.Id == id).FirstOrDefault();
        }

    }
}
