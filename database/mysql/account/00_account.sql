-- ============================================================
-- Metin2Bausia — MySQL account database (TMP4 standard schema)
-- ============================================================
CREATE DATABASE IF NOT EXISTS `account` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
USE `account`;

CREATE TABLE IF NOT EXISTS `account` (
  `id`            int(11) NOT NULL AUTO_INCREMENT,
  `login`         varchar(30) NOT NULL DEFAULT '',
  `password`      varchar(45) NOT NULL DEFAULT '',
  `social_id`     varchar(7) NOT NULL DEFAULT '',
  `email`         varchar(64) NOT NULL DEFAULT '',
  `create_time`   datetime DEFAULT NULL,
  `status`        varchar(8) NOT NULL DEFAULT 'OK',
  `available_dt`  datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `last_play`     datetime DEFAULT '0000-00-00 00:00:00',
  `cash`          int(11) unsigned NOT NULL DEFAULT 0,
  `coins`         int(11) unsigned NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  UNIQUE KEY `login` (`login`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE IF NOT EXISTS `gmlist` (
  `mAccount`  varchar(30) NOT NULL DEFAULT '',
  `mName`     varchar(32) NOT NULL DEFAULT '',
  `mGMLevel`  tinyint(4) NOT NULL DEFAULT 0,
  PRIMARY KEY (`mAccount`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- GM účet admin (heslo: 'metin2bausia' — změnit před deploym)
INSERT IGNORE INTO `account` (`login`,`password`,`social_id`,`email`,`status`,`create_time`)
  VALUES ('admin', SHA2('metin2bausia',256), '111111', 'admin@bausia.cz', 'OK', NOW());
INSERT IGNORE INTO `gmlist` VALUES ('admin', 'AdminChar', 9);
