<?php  if ( ! defined('BASEPATH')) exit('No direct script access allowed');

// -----------------------------------------------------------------------------

class MY_Model extends CI_Model
{
	/**
	 *	Insère une nouvelle ligne dans la base de données.
	 */
	public function create()
	{
		
	}

	/**
	 *	Récupère des données dans la base de données.
	 */
	public function read()
	{
		return 'read';
	}
	
	/**
	 *	Modifie une ou plusieurs lignes dans la base de données.
	 */
	public function update()
	{		
		return 'update';
	}
	
	/**
	 *	Supprime une ou plusieurs lignes de la base de données.
	 */
	public function delete()
	{
		return 'delete';
	}

	/**
	 *	Retourne le nombre de résultats.
	 */
	public function count()
	{
		return 'count';
	}
}

/* End of file MY_Model.php */
/* Location: ./system/application/libraries/MY_Model.php */