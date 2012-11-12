-- phpMyAdmin SQL Dump
-- version 3.4.9
-- http://www.phpmyadmin.net
--
-- Client: 127.0.0.1
-- Généré le : Dim 15 Avril 2012 à 20:26
-- Version du serveur: 5.5.20
-- Version de PHP: 5.3.9

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Base de données: `thegreatzcvs`
--

-- --------------------------------------------------------

--
-- Structure de la table `authors`
--

CREATE TABLE IF NOT EXISTS `authors` (
  `author_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `author_name` varchar(30) NOT NULL,
  `author_info` text NOT NULL,
  PRIMARY KEY (`author_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=5 ;

--
-- Contenu de la table `authors`
--

INSERT INTO `authors` (`author_id`, `author_name`, `author_info`) VALUES
(1, 'NE_PAS_SUPPRIMER', 'User de protection'),
(2, 'YouSS', 'Nico l''Normand'),
(3, 'Valryon', 'Dam "le semi breton"'),
(4, '1Jour1Mot', 'https://twitter.com/#!/1jour1mot');

-- --------------------------------------------------------

--
-- Structure de la table `contrepetries`
--

CREATE TABLE IF NOT EXISTS `contrepetries` (
  `ctp_id` int(11) NOT NULL AUTO_INCREMENT,
  `element_id` int(11) NOT NULL,
  `content` varchar(500) NOT NULL,
  `solution` varchar(500) NOT NULL,
  PRIMARY KEY (`ctp_id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=17 ;

--
-- Contenu de la table `contrepetries`
--

INSERT INTO `contrepetries` (`ctp_id`, `element_id`, `content`, `solution`) VALUES
(6, 18, 'Gengis Khan arrive à pied par la Chine.', 'Gengis Khan arrive à chier par la pine.'),
(9, 13, 'Le plan De Gaulle a déchiré le coeur de Massu.', 'Le gland de Paul a déchiré le cul de ma soeur.'),
(12, 15, 'Je vous laisse le choix dans la date.', 'Je vous laisse le doigt dans la chatte.'),
(13, 16, 'C''est un archéologue qui mettait dans des caisse le produit de ses fouilles.', 'C''est un archéologue qui mettait dans des fesses le produit de ses couilles.'),
(15, 17, 'Il fait beau et chaud.', 'Il fait chaud et beau.'),
(16, 14, 'Un coup de vin, c''est dimanche !', 'Un coup de manche, c''est divin !');

-- --------------------------------------------------------

--
-- Structure de la table `definitions`
--

CREATE TABLE IF NOT EXISTS `definitions` (
  `def_id` int(11) NOT NULL AUTO_INCREMENT,
  `element_id` bigint(20) NOT NULL COMMENT 'id de l''élément définis',
  `detail` text NOT NULL COMMENT 'détail des définitions',
  `content` text NOT NULL COMMENT 'content des définitions',
  PRIMARY KEY (`def_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=79 ;

--
-- Contenu de la table `definitions`
--

INSERT INTO `definitions` (`def_id`, `element_id`, `detail`, `content`) VALUES
(68, 1, 'verbe, argot', 'Tenir des propos quelconques.'),
(69, 1, 'verbe, argot', 'Parler, raconter, chercher à persuader.'),
(60, 5, 'nom masculin, vieux, populaire', 'Terme de mépris pour désigner un homme de peu et qui n''est bon à rien.'),
(61, 6, 'nom féminin', 'Jeune ouvrière coquette et galante'),
(62, 6, 'nom féminin', 'Étoffe grise de peu de valeur. // Jeune fille / femme de modeste condition qui portait cette étoffe'),
(64, 7, 'nom masculin', '[Fig.] Sonner l''hallali > Annoncer la défaite, la ruine, la faillite de quelqu''un.'),
(63, 7, 'nom masculin', '[Chasse à courre] Fanfare de trompes annonçant que l''animal poursuivi renonce à fuir. Sonner l''hallali. '),
(78, 8, 'adjectif, nom', 'Partisan du recours à la force, à la guerre pour régler des conflits. Dérivé du latin "bellicus" : guerrier.'),
(66, 9, 'littéraire', 'Personne qui présente une ressemblance frappante avec une autre. Syno. : sosie.'),
(67, 10, 'adjectif, zoologie, littré', 'Qui a une queue épaisse.'),
(76, 11, 'nom féminin, littéraire', 'Promptitude, agilité et vivacité dans les mouvements, dans les gestes.'),
(74, 12, 'nom masculin, poésie, vieux', 'Petite pièce parodique.'),
(75, 12, 'nom masculin', 'Discours, propos ou écrit involontairement confus et incompréhensible.'),
(77, 11, 'nom féminin, figuré', 'Agilité, vivacité de l''esprit.');

-- --------------------------------------------------------

--
-- Structure de la table `elements`
--

CREATE TABLE IF NOT EXISTS `elements` (
  `element_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `type_id` int(11) NOT NULL,
  `date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `title` varchar(40) NOT NULL,
  `favoriteCount` bigint(20) NOT NULL DEFAULT '0',
  `author_id` bigint(20) NOT NULL,
  UNIQUE KEY `element_id` (`element_id`),
  UNIQUE KEY `date` (`date`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=34 ;

--
-- Contenu de la table `elements`
--

INSERT INTO `elements` (`element_id`, `type_id`, `date`, `title`, `favoriteCount`, `author_id`) VALUES
(1, 1, '2012-04-29 22:00:00', 'Bonnir / bonir', 0, 4),
(18, 2, '2012-04-28 22:00:00', 'Invasion mongole', 12, 2),
(5, 1, '2012-04-22 22:00:00', 'Frélampier', 15, 4),
(6, 1, '2012-04-23 22:00:00', 'Grisette', 0, 4),
(7, 1, '2012-04-24 22:00:00', 'Hallali', 0, 4),
(8, 1, '2012-04-25 22:00:00', 'Belliciste', 0, 4),
(9, 1, '2012-04-26 22:00:00', 'Ménechme', 0, 4),
(10, 1, '2012-04-27 22:00:00', 'Crassicaude', 0, 4),
(11, 1, '2012-04-30 22:00:00', 'Prestesse', 0, 4),
(12, 1, '2012-05-01 22:00:00', 'Amphigouri', 0, 4),
(13, 2, '2012-05-05 22:00:00', '[+18] Le gaullisme  ', 10, 2),
(14, 2, '2012-04-21 22:00:00', 'Le dimanche à la messe', 0, 2),
(15, 2, '2012-05-19 22:00:00', '[+18] L''indispensable des prises de RDV', 0, 2),
(16, 2, '2012-05-26 22:00:00', '[+18] Pour le bien de l''histoire', 0, 2),
(17, 2, '2012-04-14 22:00:00', 'Contrepétrie Belge', 0, 2);

-- --------------------------------------------------------

--
-- Structure de la table `temp_tweets`
--

CREATE TABLE IF NOT EXISTS `temp_tweets` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(256) NOT NULL,
  `details` varchar(256) NOT NULL,
  `archived` bit(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Structure de la table `types`
--

CREATE TABLE IF NOT EXISTS `types` (
  `type_id` int(11) NOT NULL AUTO_INCREMENT,
  `type_name` varchar(30) NOT NULL,
  PRIMARY KEY (`type_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=3 ;

--
-- Contenu de la table `types`
--

INSERT INTO `types` (`type_id`, `type_name`) VALUES
(1, 'mot'),
(2, 'contrepétrie');

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
