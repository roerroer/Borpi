﻿angular.module('eRegistroAPI', [])
    .factory('rpiServices', ['$http', function ($http) {
        var api = '/eRegistroAPI';

        var patentes = '/Patente/';
        var marcas = '/Marca/';
        var da = '/DAutor/';
        var anota = '/Anotacion/';
        var admin = '/Admin/';

        var _marcas = {};
        var _patentes = {};
        var _anota = {};

        _marcas.getExpedienteByRegistro = function (tipoDeRegistro, registro, letra) {
            if (letra)
                return $http.get(api + marcas + 'Registro?tipoDeRegistroId=' + tipoDeRegistro + '&registro=' + registro + '&letra=' + letra);
            else
                return $http.get(api + marcas + 'Registro?tipoDeRegistroId=' + tipoDeRegistro + '&registro=' + registro);
        };

        _anota.getExpedienteDeAnotacionByNumero = function (expediente) {
            return $http.get(api + anota + 'Expediente?numero=' + expediente);
        };

        return {
            marcas: _marcas,
            anota: _anota
        };
    }]);
