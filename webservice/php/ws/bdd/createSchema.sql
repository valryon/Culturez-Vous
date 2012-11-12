-- phpMyAdmin SQL Dump
-- version OVH
-- http://www.phpmyadmin.net
--
-- Client: mysql51-55.perso
-- Généré le : Sun 25 Mars 2012 à 16:35
-- Version du serveur: 5.1.49
-- Version de PHP: 5.3.8

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

--
-- Base de données: `thegreatzcvs`
--

-- --------------------------------------------------------

--
-- Structure de la table `author`
--

CREATE TABLE IF NOT EXISTS `author` (
  `author_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `author_name` varchar(30) NOT NULL,
  `author_info` text NOT NULL,
  PRIMARY KEY (`author_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=5 ;

-- --------------------------------------------------------

--
-- Structure de la table `ci_sessions`
--

CREATE TABLE IF NOT EXISTS `ci_sessions` (
  `session_id` varchar(40) NOT NULL DEFAULT '0',
  `ip_address` varchar(16) NOT NULL DEFAULT '0',
  `user_agent` varchar(120) NOT NULL,
  `last_activity` int(10) unsigned NOT NULL DEFAULT '0',
  `user_data` text NOT NULL,
  PRIMARY KEY (`session_id`),
  KEY `last_activity_idx` (`last_activity`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `definition`
--

CREATE TABLE IF NOT EXISTS `definition` (
  `element_id` bigint(20) NOT NULL COMMENT 'id de l''élément définis',
  `definition_details` text NOT NULL COMMENT 'détail des définitions',
  `definition_content` text NOT NULL COMMENT 'content des définitions'
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Structure de la table `element`
--

CREATE TABLE IF NOT EXISTS `element` (
  `element_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `type_id` int(11) NOT NULL,
  `element_date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `element_title` varchar(40) NOT NULL,
  `element_favoriteCount` bigint(20) NOT NULL DEFAULT '0',
  `element_preview` text NOT NULL,
  `element_solution` text,
  `author_id` bigint(20) NOT NULL,
  UNIQUE KEY `element_id` (`element_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=6 ;

-- --------------------------------------------------------

--
-- Structure de la table `type`
--

CREATE TABLE IF NOT EXISTS `type` (
  `type_id` int(11) NOT NULL AUTO_INCREMENT,
  `type_name` varchar(30) NOT NULL,
  PRIMARY KEY (`type_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=3 ;
