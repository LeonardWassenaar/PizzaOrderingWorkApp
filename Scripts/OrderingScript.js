var order = angular.module('OrderingScript', [])
order.controller('orderControl', function ($scope, $http) {
    $scope.orders = "";
    $http({
        method: 'get',
        url: '/Order/getOrder',
        dataType: "json",
    }).then(function successCallback(response) {
        $scope.orders = response.data;
    }, function errorCallback(response) {
        alert(response);
    });
});