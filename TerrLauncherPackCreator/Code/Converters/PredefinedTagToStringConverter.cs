﻿using System;
using System.Globalization;
using System.Windows.Data;
using CrossPlatform.Code.Enums;
using CrossPlatform.Code.Utils;
using TerrLauncherPackCreator.Resources.Localizations;

namespace TerrLauncherPackCreator.Code.Converters;

public class PredefinedTagToStringConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        (1 / (32 / PredefinedPackTagEnum.Length)).Ignore();

#pragma warning disable CS8524
        return (value as PredefinedPackTag?) switch
#pragma warning restore CS8524
        {
            PredefinedPackTag.TexturesAnimated => StringResources.PredefinedPackTagTexturesAnimated,
            PredefinedPackTag.TexturesWeapons => StringResources.PredefinedPackTagTexturesWeapons,
            PredefinedPackTag.TexturesTools => StringResources.PredefinedPackTagTexturesTools,
            PredefinedPackTag.TexturesVanity => StringResources.PredefinedPackTagTexturesVanity,
            PredefinedPackTag.TexturesArmor => StringResources.PredefinedPackTagTexturesArmor,
            PredefinedPackTag.TexturesPets => StringResources.PredefinedPackTagTexturesPets,
            PredefinedPackTag.TexturesBosses => StringResources.PredefinedPackTagTexturesBosses,
            PredefinedPackTag.TexturesMobs => StringResources.PredefinedPackTagTexturesMobs,
            PredefinedPackTag.TexturesNpc => StringResources.PredefinedPackTagTexturesNpc,
            PredefinedPackTag.TexturesBlocks => StringResources.PredefinedPackTagTexturesBlocks,
            PredefinedPackTag.TexturesOther => StringResources.PredefinedPackTagTexturesOther,
            PredefinedPackTag.MapsBuildings => StringResources.PredefinedPackTagMapsBuildings,
            PredefinedPackTag.MapsAdventure => StringResources.PredefinedPackTagMapsAdventure,
            PredefinedPackTag.MapsSurvival => StringResources.PredefinedPackTagMapsSurvival,
            PredefinedPackTag.MapsOther => StringResources.PredefinedPackTagMapsOther,
            PredefinedPackTag.MapsParkour => StringResources.PredefinedPackTagMapsParkour,
            PredefinedPackTag.MapsForMultiplePlayers => StringResources.PredefinedPackTagMapsForMultiplePlayers,
            PredefinedPackTag.CharactersCombat => StringResources.PredefinedPackTagCharactersCombat,
            PredefinedPackTag.CharactersAppearance => StringResources.PredefinedPackTagCharactersAppearance,
            PredefinedPackTag.CharactersOther => StringResources.PredefinedPackTagCharactersOther,
            PredefinedPackTag.GuiAnimated => StringResources.PredefinedPackTagGuiAnimated,
            PredefinedPackTag.GuiInventory => StringResources.PredefinedPackTagGuiInventory,
            PredefinedPackTag.GuiHealthOrMana => StringResources.PredefinedPackTagGuiHealthOrMana,
            PredefinedPackTag.GuiGeneral => StringResources.PredefinedPackTagGuiGeneral,
            PredefinedPackTag.GuiOther => StringResources.PredefinedPackTagGuiOther,
            PredefinedPackTag.AudioBiomsOrLocation => StringResources.PredefinedPackTagAudioBiomsOrLocations,
            PredefinedPackTag.AudioBosses => StringResources.PredefinedPackTagAudioBosses,
            PredefinedPackTag.AudioEvents => StringResources.PredefinedPackTagAudioEvents,
            PredefinedPackTag.AudioSounds => StringResources.PredefinedPackTagAudioSounds,
            PredefinedPackTag.AudioOther => StringResources.PredefinedPackTagAudioOther,
            PredefinedPackTag.FontsAnimated => StringResources.PredefinedPackTagFontsAnimated,
            PredefinedPackTag.ModsScripts => StringResources.PredefinedPackTagModsScripts
        };
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string val)
            return null;

        (1 / (32 / PredefinedPackTagEnum.Length)).Ignore();

        if (val == StringResources.PredefinedPackTagTexturesAnimated)
            return PredefinedPackTag.TexturesAnimated;
        if (val == StringResources.PredefinedPackTagTexturesWeapons)
            return PredefinedPackTag.TexturesWeapons;
        if (val == StringResources.PredefinedPackTagTexturesTools)
            return PredefinedPackTag.TexturesTools;
        if (val == StringResources.PredefinedPackTagTexturesVanity)
            return PredefinedPackTag.TexturesVanity;
        if (val == StringResources.PredefinedPackTagTexturesArmor)
            return PredefinedPackTag.TexturesArmor;
        if (val == StringResources.PredefinedPackTagTexturesPets)
            return PredefinedPackTag.TexturesPets;
        if (val == StringResources.PredefinedPackTagTexturesBosses)
            return PredefinedPackTag.AudioBosses;
        if (val == StringResources.PredefinedPackTagTexturesMobs)
            return PredefinedPackTag.TexturesMobs;
        if (val == StringResources.PredefinedPackTagTexturesNpc)
            return PredefinedPackTag.TexturesNpc;
        if (val == StringResources.PredefinedPackTagTexturesBlocks)
            return PredefinedPackTag.TexturesBlocks;
        if (val == StringResources.PredefinedPackTagTexturesOther)
            return PredefinedPackTag.TexturesOther;
        if (val == StringResources.PredefinedPackTagMapsBuildings)
            return PredefinedPackTag.MapsBuildings;
        if (val == StringResources.PredefinedPackTagMapsAdventure)
            return PredefinedPackTag.MapsAdventure;
        if (val == StringResources.PredefinedPackTagMapsSurvival)
            return PredefinedPackTag.MapsSurvival;
        if (val == StringResources.PredefinedPackTagMapsOther)
            return PredefinedPackTag.MapsOther;
        if (val == StringResources.PredefinedPackTagMapsParkour)
            return PredefinedPackTag.MapsParkour;
        if (val == StringResources.PredefinedPackTagMapsForMultiplePlayers)
            return PredefinedPackTag.MapsForMultiplePlayers;
        if (val == StringResources.PredefinedPackTagCharactersCombat)
            return PredefinedPackTag.CharactersCombat;
        if (val == StringResources.PredefinedPackTagCharactersAppearance)
            return PredefinedPackTag.CharactersAppearance;
        if (val == StringResources.PredefinedPackTagCharactersOther)
            return PredefinedPackTag.CharactersOther;
        if (val == StringResources.PredefinedPackTagGuiAnimated)
            return PredefinedPackTag.GuiAnimated;
        if (val == StringResources.PredefinedPackTagGuiInventory)
            return PredefinedPackTag.GuiInventory;
        if (val == StringResources.PredefinedPackTagGuiHealthOrMana)
            return PredefinedPackTag.GuiHealthOrMana;
        if (val == StringResources.PredefinedPackTagGuiGeneral)
            return PredefinedPackTag.GuiGeneral;
        if (val == StringResources.PredefinedPackTagGuiOther)
            return PredefinedPackTag.GuiOther;
        if (val == StringResources.PredefinedPackTagAudioBiomsOrLocations)
            return PredefinedPackTag.AudioBiomsOrLocation;
        if (val == StringResources.PredefinedPackTagAudioBosses)
            return PredefinedPackTag.AudioBosses;
        if (val == StringResources.PredefinedPackTagAudioEvents)
            return PredefinedPackTag.AudioEvents;
        if (val == StringResources.PredefinedPackTagAudioSounds)
            return PredefinedPackTag.AudioSounds;
        if (val == StringResources.PredefinedPackTagAudioOther)
            return PredefinedPackTag.AudioOther;
        if (val == StringResources.PredefinedPackTagFontsAnimated)
            return PredefinedPackTag.FontsAnimated;
        if (val == StringResources.PredefinedPackTagModsScripts)
            return PredefinedPackTag.ModsScripts;
        return null;
    }
}