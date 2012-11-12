<?php  if ( ! defined('BASEPATH')) exit('No direct script access allowed');

// -----------------------------------------------------------------------------

class MY_Model extends CI_Model
{
	/**
	 *	Ins�re une nouvelle ligne dans la base de donn�es.
	 */
	public function create()
	{
		
	}

	/**
	 *	R�cup�re des donn�es dans la base de donn�es.
	 */
	public function read()
	{
		return 'read';
	}
	
	/**
	 *	Modifie une ou plusieurs lignes dans la base de donn�es.
	 */
	public function update()
	{		
		return 'update';
	}
	
	/**
	 *	Supprime une ou plusieurs lignes de la base de donn�es.
	 */
	public function delete()
	{
		return 'delete';
	}

	/**
	 *	Retourne le nombre de r�sultats.
	 */
	public function count()
	{
		return 'count';
	}
}

/* End of file MY_Model.php */
/* Location: ./system/application/libraries/MY_Model.php */