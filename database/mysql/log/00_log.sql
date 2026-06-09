-- ============================================================
-- Metin2Bausia — MySQL log database (TMP4 standard)
-- ============================================================

CREATE DATABASE IF NOT EXISTS log CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE log;

-- Obecný log
CREATE TABLE IF NOT EXISTS log (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT,
    `time`      DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
    who         INT NOT NULL DEFAULT 0,
    how         INT NOT NULL DEFAULT 0,
    what        INT NOT NULL DEFAULT 0,
    item_vnum   INT UNSIGNED NOT NULL DEFAULT 0,
    item_count  TINYINT UNSIGNED NOT NULL DEFAULT 0,
    PRIMARY KEY (id),
    KEY `time` (`time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Log příkazů GM
CREATE TABLE IF NOT EXISTS gmlog (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT,
    `time`      DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
    gm          VARCHAR(32) NOT NULL DEFAULT '',
    `explain`   VARCHAR(200) NOT NULL DEFAULT '',
    ip          VARCHAR(16) NOT NULL DEFAULT '',
    vnum        INT UNSIGNED NOT NULL DEFAULT 0,
    count       INT NOT NULL DEFAULT 0,
    gold        INT NOT NULL DEFAULT 0,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Log připojení hráčů
CREATE TABLE IF NOT EXISTS loginlog (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT,
    `time`      DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
    ip          VARCHAR(16) NOT NULL DEFAULT '',
    account     VARCHAR(30) NOT NULL DEFAULT '',
    name        VARCHAR(24) NOT NULL DEFAULT '',
    PRIMARY KEY (id),
    KEY account (account)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Log yangů (ekonomika)
CREATE TABLE IF NOT EXISTS goldlog (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT,
    `time`      DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00,',
    pid         INT UNSIGNED NOT NULL DEFAULT 0,
    name        VARCHAR(24) NOT NULL DEFAULT '',
    delta       BIGINT NOT NULL DEFAULT 0,
    total       BIGINT NOT NULL DEFAULT 0,
    type        TINYINT NOT NULL DEFAULT 0,
    PRIMARY KEY (id),
    KEY pid (pid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Log questů
CREATE TABLE IF NOT EXISTS questlog (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT,
    `time`      DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
    pid         INT UNSIGNED NOT NULL DEFAULT 0,
    name        VARCHAR(24) NOT NULL DEFAULT '',
    quest_name  VARCHAR(128) NOT NULL DEFAULT '',
    state       VARCHAR(64) NOT NULL DEFAULT '',
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Log craftingu
CREATE TABLE IF NOT EXISTS craftlog (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT,
    `time`      DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
    pid         INT UNSIGNED NOT NULL DEFAULT 0,
    name        VARCHAR(24) NOT NULL DEFAULT '',
    result_vnum INT UNSIGNED NOT NULL DEFAULT 0,
    result_count TINYINT NOT NULL DEFAULT 0,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Log obchodů (trade)
CREATE TABLE IF NOT EXISTS tradelog (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT,
    `time`      DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
    pid1        INT UNSIGNED NOT NULL DEFAULT 0,
    name1       VARCHAR(24) NOT NULL DEFAULT '',
    pid2        INT UNSIGNED NOT NULL DEFAULT 0,
    name2       VARCHAR(24) NOT NULL DEFAULT '',
    item_vnum   INT UNSIGNED NOT NULL DEFAULT 0,
    item_count  TINYINT NOT NULL DEFAULT 0,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
