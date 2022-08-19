SELECT `c`.`Id`, `c`.`Active`, `c`.`Affection`, `c`.`Attack`, `c`.`AttackBonus`, `c`.`CardPower`, `c`.`CharacterId`, `c`.`CreatedOn`, `c`.`Curse`, `c`.`CustomBorderUrl`, `c`.`CustomImageUrl`, `c`.`Defence`, `c`.`DefenceBonus`, `c`.`Dere`, `c`.`EnhanceCount`, `c`.`Expedition`, `c`.`ExpeditionDate`, `c`.`ExperienceCount`, `c`.`FirstOwnerId`, `c`.`FromFigure`, `c`.`GameDeckId`, `c`.`Health`, `c`.`HealthBonus`, `c`.`ImageUrl`, `c`.`InCage`, `c`.`IsTradable`, `c`.`IsUnique`, `c`.`LastOwnerId`, `c`.`MarketValue`, `c`.`Name`, `c`.`PAS`, `c`.`Quality`, `c`.`QualityOnStart`, `c`.`Rarity`, `c`.`RarityOnStart`, `c`.`RestartCount`, `c`.`Source`, `c`.`StarStyle`, `c`.`Title`, `c`.`UpgradesCount`
FROM `Cards` AS `c`
WHERE `c`.`CharacterId` = 1

