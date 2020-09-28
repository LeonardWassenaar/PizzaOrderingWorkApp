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

    $scope.savedata = function (_order) {
        debugger;
        $http({
            method: 'post',
            url: '/Order/saveOrder',
            dataType: "json",
            data: { order: _order} 
        }).then(function successCallback(response) {
            debugger;
            $scope.orders = response.data;
        }, function errorCallback(response) {
            alert(response);
        })
    }      
});