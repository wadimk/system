var lib = require('lib');
var lang = require('lang!static/web-ui/apps/lang.json');
var layoutTemplate = require('static/web-ui/apps/list.tpl');
var itemTemplate = require('static/web-ui/apps/list-item.tpl');

//#region entities

var SectionModel = lib.backbone.Model.extend({
    defaults: {
        sortOrder: 0
    }
});

var SectionCollection = lib.backbone.Collection.extend({
    model: SectionModel,
    comparator: 'sortOrder'
});

//#endregion

//#region views

var ItemView = lib.marionette.View.extend({
    template: lib.handlebars.compile(itemTemplate),
    tagName: 'li',
    className: 'th-list-item',  
    triggers: {
	    'click .js-section-link': 'navigate'
    }

});

var ListView = lib.marionette.CollectionView.extend({
	childView: ItemView,
	className: 'list-unstyled',
    tagName: 'ul',
	childViewTriggers: {
        'navigate': 'childview:navigate'
	}
});

var LayoutView = lib.marionette.View.extend({
    template: lib.handlebars.compile(layoutTemplate),
    regions: {
        list: '.js-list'
    },
    onRender: function() {
        this.$('.js-title').text(this.getOption('title'));
    }
});

//#endregion

var Section = lib.common.AppSection.extend({
    title: lang.get('Applications'),
    url: '/api/web-ui/apps/user',
    start: function() {
        this.view = new LayoutView({
            title: this.getOption('title')
        });

        this.application.setContentView(this.view);

        return lib.ajax
            .loadModel(this.getOption('url'), SectionCollection)
            .then(this.bind('displayList'));
    },
    displayList: function (items) {
        var listView = new ListView({ collection: items });
        this.listenTo(listView, 'childview:navigate', this.bind('onSectionSelect'));

        this.view.showChildView('list', listView);
    },
    onSectionSelect: function(childView) {
        var url = childView.model.get('url');
        this.application.navigate(url);
    }

});

module.exports = Section;
