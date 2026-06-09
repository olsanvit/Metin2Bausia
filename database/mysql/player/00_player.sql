-- ============================================================
-- Metin2Bausia — MySQL player database (TMP4 standard)
-- ============================================================

CREATE DATABASE IF NOT EXISTS player CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE player;

-- Hlavní tabulka postav
CREATE TABLE IF NOT EXISTS player (
    id              INT UNSIGNED NOT NULL AUTO_INCREMENT,
    account_id      INT UNSIGNED NOT NULL DEFAULT 0,
    name            VARCHAR(24) NOT NULL DEFAULT '',
    job             TINYINT UNSIGNED NOT NULL DEFAULT 0,
    voice           TINYINT UNSIGNED NOT NULL DEFAULT 0,
    dir             TINYINT NOT NULL DEFAULT 0,
    x               INT NOT NULL DEFAULT 0,
    y               INT NOT NULL DEFAULT 0,
    z               INT NOT NULL DEFAULT 0,
    map_index       INT NOT NULL DEFAULT 0,
    exit_x          INT NOT NULL DEFAULT 0,
    exit_y          INT NOT NULL DEFAULT 0,
    exit_map_index  INT NOT NULL DEFAULT 0,
    hp              INT NOT NULL DEFAULT 0,
    mp              INT NOT NULL DEFAULT 0,
    stamina         INT NOT NULL DEFAULT 0,
    random_hp       INT NOT NULL DEFAULT 0,
    random_sp       INT NOT NULL DEFAULT 0,
    playtime        INT UNSIGNED NOT NULL DEFAULT 0,
    level           SMALLINT NOT NULL DEFAULT 1,
    st              SMALLINT NOT NULL DEFAULT 0,
    ht              SMALLINT NOT NULL DEFAULT 0,
    dx              SMALLINT NOT NULL DEFAULT 0,
    iq              SMALLINT NOT NULL DEFAULT 0,
    exp             INT UNSIGNED NOT NULL DEFAULT 0,
    gold            BIGINT NOT NULL DEFAULT 0,
    skill_point     INT NOT NULL DEFAULT 0,
    sub_skill_point INT NOT NULL DEFAULT 0,
    stat_point      INT NOT NULL DEFAULT 0,
    empire          TINYINT NOT NULL DEFAULT 0,
    part_main       SMALLINT NOT NULL DEFAULT 0,
    part_hair       SMALLINT NOT NULL DEFAULT 0,
    part_sash       SMALLINT NOT NULL DEFAULT 0,
    skill_group     TINYINT NOT NULL DEFAULT 0,
    available_ride  TINYINT NOT NULL DEFAULT 0,
    last_play       DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
    create_time     DATETIME NOT NULL DEFAULT '0000-00-00 00:00:00',
    PRIMARY KEY (id),
    UNIQUE KEY name (name),
    KEY account_id (account_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Inventář postav
CREATE TABLE IF NOT EXISTS item (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT,
    owner_id    INT UNSIGNED NOT NULL DEFAULT 0,
    window      TINYINT NOT NULL DEFAULT 0,
    pos         SMALLINT NOT NULL DEFAULT 0,
    count       TINYINT UNSIGNED NOT NULL DEFAULT 1,
    vnum        INT UNSIGNED NOT NULL DEFAULT 0,
    socket0     INT NOT NULL DEFAULT 0,
    socket1     INT NOT NULL DEFAULT 0,
    socket2     INT NOT NULL DEFAULT 0,
    socket3     INT NOT NULL DEFAULT 0,
    socket4     INT NOT NULL DEFAULT 0,
    socket5     INT NOT NULL DEFAULT 0,
    attrtype0   TINYINT UNSIGNED NOT NULL DEFAULT 0,
    attrvalue0  SMALLINT NOT NULL DEFAULT 0,
    attrtype1   TINYINT UNSIGNED NOT NULL DEFAULT 0,
    attrvalue1  SMALLINT NOT NULL DEFAULT 0,
    attrtype2   TINYINT UNSIGNED NOT NULL DEFAULT 0,
    attrvalue2  SMALLINT NOT NULL DEFAULT 0,
    attrtype3   TINYINT UNSIGNED NOT NULL DEFAULT 0,
    attrvalue3  SMALLINT NOT NULL DEFAULT 0,
    attrtype4   TINYINT UNSIGNED NOT NULL DEFAULT 0,
    attrvalue4  SMALLINT NOT NULL DEFAULT 0,
    attrtype5   TINYINT UNSIGNED NOT NULL DEFAULT 0,
    attrvalue5  SMALLINT NOT NULL DEFAULT 0,
    attrtype6   TINYINT UNSIGNED NOT NULL DEFAULT 0,
    attrvalue6  SMALLINT NOT NULL DEFAULT 0,
    PRIMARY KEY (id),
    KEY owner_id (owner_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Skill tabulka postav
CREATE TABLE IF NOT EXISTS skill (
    player_id   INT UNSIGNED NOT NULL DEFAULT 0,
    skill_vnum  SMALLINT UNSIGNED NOT NULL DEFAULT 0,
    skill_level TINYINT NOT NULL DEFAULT 0,
    PRIMARY KEY (player_id, skill_vnum)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Questová tabulka postav
CREATE TABLE IF NOT EXISTS quest (
    dwPID       INT UNSIGNED NOT NULL DEFAULT 0,
    szName      VARCHAR(128) NOT NULL DEFAULT '',
    lState      INT NOT NULL DEFAULT 0,
    szFlag      VARCHAR(128) NOT NULL DEFAULT '',
    lValue      INT NOT NULL DEFAULT 0,
    PRIMARY KEY (dwPID, szName, szFlag)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Přátelé
CREATE TABLE IF NOT EXISTS friend (
    pid         INT UNSIGNED NOT NULL DEFAULT 0,
    companion   INT UNSIGNED NOT NULL DEFAULT 0,
    name        VARCHAR(24) NOT NULL DEFAULT '',
    PRIMARY KEY (pid, companion)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Ignore list
CREATE TABLE IF NOT EXISTS messenger_block (
    account_id  INT UNSIGNED NOT NULL DEFAULT 0,
    target_id   INT UNSIGNED NOT NULL DEFAULT 0,
    PRIMARY KEY (account_id, target_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Guild tabulka
CREATE TABLE IF NOT EXISTS guild (
    id          INT UNSIGNED NOT NULL AUTO_INCREMENT,
    name        VARCHAR(12) NOT NULL DEFAULT '',
    master      INT UNSIGNED NOT NULL DEFAULT 0,
    exp         INT UNSIGNED NOT NULL DEFAULT 0,
    level       TINYINT UNSIGNED NOT NULL DEFAULT 0,
    skill_point TINYINT UNSIGNED NOT NULL DEFAULT 0,
    win         INT UNSIGNED NOT NULL DEFAULT 0,
    draw        INT UNSIGNED NOT NULL DEFAULT 0,
    loss        INT UNSIGNED NOT NULL DEFAULT 0,
    ladder_point INT NOT NULL DEFAULT 0,
    gold        INT NOT NULL DEFAULT 0,
    PRIMARY KEY (id),
    UNIQUE KEY name (name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Guild members
CREATE TABLE IF NOT EXISTS guild_member (
    guild_id    INT UNSIGNED NOT NULL DEFAULT 0,
    pid         INT UNSIGNED NOT NULL DEFAULT 0,
    grade       TINYINT UNSIGNED NOT NULL DEFAULT 0,
    is_general  TINYINT UNSIGNED NOT NULL DEFAULT 0,
    offer       INT UNSIGNED NOT NULL DEFAULT 0,
    PRIMARY KEY (guild_id, pid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Warehouse (private store)
CREATE TABLE IF NOT EXISTS safebox (
    id          INT UNSIGNED NOT NULL DEFAULT 0,
    size        TINYINT UNSIGNED NOT NULL DEFAULT 1,
    password    VARCHAR(6) NOT NULL DEFAULT '',
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Offline shop
CREATE TABLE IF NOT EXISTS offline_shop (
    owner_id    INT UNSIGNED NOT NULL DEFAULT 0,
    shop_name   VARCHAR(32) NOT NULL DEFAULT '',
    money       BIGINT NOT NULL DEFAULT 0,
    PRIMARY KEY (owner_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Horse
CREATE TABLE IF NOT EXISTS horse (
    player_id   INT UNSIGNED NOT NULL DEFAULT 0,
    level       TINYINT UNSIGNED NOT NULL DEFAULT 0,
    `riding`    TINYINT UNSIGNED NOT NULL DEFAULT 0,
    hp          SMALLINT UNSIGNED NOT NULL DEFAULT 0,
    stamina     SMALLINT UNSIGNED NOT NULL DEFAULT 0,
    PRIMARY KEY (player_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
