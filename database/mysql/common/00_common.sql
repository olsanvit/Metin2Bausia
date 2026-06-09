-- ============================================================
-- Metin2Bausia — MySQL common database (TMP4 standard schema)
-- item_proto, mob_proto, skill_proto — zdroj pravdy pro herní server
-- ============================================================
CREATE DATABASE IF NOT EXISTS `common` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
USE `common`;

-- item_proto: definice všech itemů (synchronizováno z PostgreSQL Items kde ContentStatus='approved')
CREATE TABLE IF NOT EXISTS `item_proto` (
  `vnum`          int(11) unsigned NOT NULL,
  `name`          varchar(24) NOT NULL DEFAULT '',
  `locale_name`   varchar(24) NOT NULL DEFAULT '',
  `type`          tinyint(4) NOT NULL DEFAULT 0,
  `subtype`       tinyint(4) NOT NULL DEFAULT 0,
  `weight`        tinyint(4) unsigned NOT NULL DEFAULT 0,
  `size`          tinyint(4) unsigned NOT NULL DEFAULT 0,
  `anti_flags`    int(11) unsigned NOT NULL DEFAULT 0,
  `flags`         int(11) unsigned NOT NULL DEFAULT 0,
  `wear_flags`    int(11) unsigned NOT NULL DEFAULT 0,
  `immune_flags`  int(11) unsigned NOT NULL DEFAULT 0,
  `gold`          int(11) unsigned NOT NULL DEFAULT 0,
  `buy`           int(11) unsigned NOT NULL DEFAULT 0,
  `limit_type0`   tinyint(4) NOT NULL DEFAULT 0,
  `limit_value0`  int(11) NOT NULL DEFAULT 0,
  `limit_type1`   tinyint(4) NOT NULL DEFAULT 0,
  `limit_value1`  int(11) NOT NULL DEFAULT 0,
  `apply_type0`   tinyint(4) NOT NULL DEFAULT 0,
  `apply_value0`  int(11) NOT NULL DEFAULT 0,
  `apply_type1`   tinyint(4) NOT NULL DEFAULT 0,
  `apply_value1`  int(11) NOT NULL DEFAULT 0,
  `apply_type2`   tinyint(4) NOT NULL DEFAULT 0,
  `apply_value2`  int(11) NOT NULL DEFAULT 0,
  `value0`        int(11) NOT NULL DEFAULT 0,
  `value1`        int(11) NOT NULL DEFAULT 0,
  `value2`        int(11) NOT NULL DEFAULT 0,
  `value3`        int(11) NOT NULL DEFAULT 0,
  `value4`        int(11) NOT NULL DEFAULT 0,
  `value5`        int(11) NOT NULL DEFAULT 0,
  `socket0`       int(11) NOT NULL DEFAULT 0,
  `socket1`       int(11) NOT NULL DEFAULT 0,
  `socket2`       int(11) NOT NULL DEFAULT 0,
  `specular`      tinyint(4) NOT NULL DEFAULT 0,
  `socket_pct`    tinyint(4) NOT NULL DEFAULT 0,
  `addon_type`    smallint(6) NOT NULL DEFAULT 0,
  PRIMARY KEY (`vnum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- mob_proto: definice všech mobů/npc (synchronizováno z PostgreSQL Mobs kde ContentStatus='approved')
CREATE TABLE IF NOT EXISTS `mob_proto` (
  `vnum`              int(11) unsigned NOT NULL,
  `name`              varchar(24) NOT NULL DEFAULT '',
  `locale_name`       varchar(24) NOT NULL DEFAULT '',
  `type`              tinyint(4) NOT NULL DEFAULT 0,
  `rank`              tinyint(4) NOT NULL DEFAULT 0,
  `battle_type`       tinyint(4) NOT NULL DEFAULT 0,
  `level`             tinyint(4) NOT NULL DEFAULT 0,
  `size`              tinyint(4) unsigned NOT NULL DEFAULT 0,
  `gold_min`          int(11) unsigned NOT NULL DEFAULT 0,
  `gold_max`          int(11) unsigned NOT NULL DEFAULT 0,
  `exp`               int(11) unsigned NOT NULL DEFAULT 0,
  `max_hp`            int(11) NOT NULL DEFAULT 0,
  `regen_cycle`       tinyint(4) unsigned NOT NULL DEFAULT 0,
  `regen_pct`         tinyint(4) unsigned NOT NULL DEFAULT 0,
  `gold_drop_pct`     tinyint(4) unsigned NOT NULL DEFAULT 0,
  `def`               smallint(6) unsigned NOT NULL DEFAULT 0,
  `attack_speed`      smallint(6) NOT NULL DEFAULT 0,
  `move_speed`        smallint(6) NOT NULL DEFAULT 0,
  `aggressive_hp_pct` tinyint(4) unsigned NOT NULL DEFAULT 0,
  `aggressive_sight`  smallint(6) unsigned NOT NULL DEFAULT 0,
  `attack_range`      smallint(6) unsigned NOT NULL DEFAULT 0,
  `scale_percent`     tinyint(4) unsigned NOT NULL DEFAULT 100,
  `atk`               int(11) NOT NULL DEFAULT 0,
  `magic_atk`         int(11) NOT NULL DEFAULT 0,
  `magic_def`         int(11) NOT NULL DEFAULT 0,
  `immune_flags`      int(11) unsigned NOT NULL DEFAULT 0,
  `res_human`         tinyint(4) NOT NULL DEFAULT 0,
  `res_animal`        tinyint(4) NOT NULL DEFAULT 0,
  `res_orc`           tinyint(4) NOT NULL DEFAULT 0,
  `res_milgyo`        tinyint(4) NOT NULL DEFAULT 0,
  `res_undead`        tinyint(4) NOT NULL DEFAULT 0,
  `res_devil`         tinyint(4) NOT NULL DEFAULT 0,
  `res_fire`          tinyint(4) NOT NULL DEFAULT 0,
  `res_elect`         tinyint(4) NOT NULL DEFAULT 0,
  `res_magic`         tinyint(4) NOT NULL DEFAULT 0,
  `res_wind`          tinyint(4) NOT NULL DEFAULT 0,
  `res_poison`        tinyint(4) NOT NULL DEFAULT 0,
  `res_bow`           tinyint(4) NOT NULL DEFAULT 0,
  PRIMARY KEY (`vnum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- skill_proto
CREATE TABLE IF NOT EXISTS `skill_proto` (
  `dwVnum`              int(11) unsigned NOT NULL,
  `szName`              varchar(32) NOT NULL DEFAULT '',
  `szLocaleName`        varchar(32) NOT NULL DEFAULT '',
  `bType`               tinyint(4) NOT NULL DEFAULT 0,
  `bLevelStep`          tinyint(4) NOT NULL DEFAULT 0,
  `bMaxLevel`           tinyint(4) NOT NULL DEFAULT 20,
  `dwTargetFlag`        int(11) NOT NULL DEFAULT 0,
  `dwAffectFlag`        bigint(20) NOT NULL DEFAULT 0,
  `szMotion`            varchar(64) NOT NULL DEFAULT '',
  `szSfxDuration`       varchar(64) NOT NULL DEFAULT '',
  `szSfxFire`           varchar(64) NOT NULL DEFAULT '',
  `szSfxHit`            varchar(64) NOT NULL DEFAULT '',
  `szSfxSpecial`        varchar(64) NOT NULL DEFAULT '',
  `szSfxProjectile`     varchar(64) NOT NULL DEFAULT '',
  `szSfxProjectileEnd`  varchar(64) NOT NULL DEFAULT '',
  PRIMARY KEY (`dwVnum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
