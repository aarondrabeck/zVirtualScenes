Ext.define('Ext.event.ListenerStack',{currentOrder:'current',length:0,constructor:function(){this.listeners={before:[],current:[],after:[]};this.lateBindingMap={};return this;},add:function(fn,scope,options,order){var lateBindingMap=this.lateBindingMap,listeners=this.getAll(order),i=listeners.length,bindingMap,listener,id;if(typeof fn=='string'&&scope.isIdentifiable){id=scope.getId();bindingMap=lateBindingMap[id];if(bindingMap){if(bindingMap[fn]){return false;}
else{bindingMap[fn]=true;}}
else{lateBindingMap[id]=bindingMap={};bindingMap[fn]=true;}}
else{if(i>0){while(i--){listener=listeners[i];if(listener.fn===fn&&listener.scope===scope){listener.options=options;return false;}}}}
listener=this.create(fn,scope,options,order);if(options&&options.prepend){delete options.prepend;listeners.unshift(listener);}
else{listeners.push(listener);}
this.length++;return true;},getAt:function(index,order){return this.getAll(order)[index];},getAll:function(order){if(!order){order=this.currentOrder;}
return this.listeners[order];},count:function(order){return this.getAll(order).length;},create:function(fn,scope,options,order){return{stack:this,fn:fn,firingFn:false,boundFn:false,isLateBinding:typeof fn=='string',scope:scope,options:options||{},order:order};},remove:function(fn,scope,order){var listeners=this.getAll(order),i=listeners.length,isRemoved=false,lateBindingMap=this.lateBindingMap,listener,id;if(i>0){while(i--){listener=listeners[i];if(listener.fn===fn&&listener.scope===scope){listeners.splice(i,1);isRemoved=true;this.length--;if(typeof fn=='string'&&scope.isIdentifiable){id=scope.getId();if(lateBindingMap[id]&&lateBindingMap[id][fn]){delete lateBindingMap[id][fn];}}
break;}}}
return isRemoved;}});Ext.define('Ext.event.Controller',{isFiring:false,listenerStack:null,constructor:function(info){this.firingListeners=[];this.firingArguments=[];this.setInfo(info);return this;},setInfo:function(info){this.info=info;},getInfo:function(){return this.info;},setListenerStacks:function(listenerStacks){this.listenerStacks=listenerStacks;},fire:function(args,action){var listenerStacks=this.listenerStacks,firingListeners=this.firingListeners,firingArguments=this.firingArguments,push=firingListeners.push,ln=listenerStacks.length,listeners,beforeListeners,currentListeners,afterListeners,isActionBefore=false,isActionAfter=false,i;firingListeners.length=0;if(action){if(action.order!=='after'){isActionBefore=true;}
else{isActionAfter=true;}}
if(ln===1){listeners=listenerStacks[0].listeners;beforeListeners=listeners.before;currentListeners=listeners.current;afterListeners=listeners.after;if(beforeListeners.length>0){push.apply(firingListeners,beforeListeners);}
if(isActionBefore){push.call(firingListeners,action);}
if(currentListeners.length>0){push.apply(firingListeners,currentListeners);}
if(isActionAfter){push.call(firingListeners,action);}
if(afterListeners.length>0){push.apply(firingListeners,afterListeners);}}
else{for(i=0;i<ln;i++){beforeListeners=listenerStacks[i].listeners.before;if(beforeListeners.length>0){push.apply(firingListeners,beforeListeners);}}
if(isActionBefore){push.call(firingListeners,action);}
for(i=0;i<ln;i++){currentListeners=listenerStacks[i].listeners.current;if(currentListeners.length>0){push.apply(firingListeners,currentListeners);}}
if(isActionAfter){push.call(firingListeners,action);}
for(i=0;i<ln;i++){afterListeners=listenerStacks[i].listeners.after;if(afterListeners.length>0){push.apply(firingListeners,afterListeners);}}}
if(firingListeners.length===0){return this;}
if(!args){args=[];}
firingArguments.length=0;firingArguments.push.apply(firingArguments,args);firingArguments.push(null,this);this.doFire();return this;},doFire:function(){var firingListeners=this.firingListeners,firingArguments=this.firingArguments,optionsArgumentIndex=firingArguments.length-2,i,ln,listener,options,fn,firingFn,boundFn,isLateBinding,scope,args,result;this.isPausing=false;this.isPaused=false;this.isStopped=false;this.isFiring=true;for(i=0,ln=firingListeners.length;i<ln;i++){listener=firingListeners[i];options=listener.options;fn=listener.fn;firingFn=listener.firingFn;boundFn=listener.boundFn;isLateBinding=listener.isLateBinding;scope=listener.scope;if(isLateBinding&&boundFn&&boundFn!==scope[fn]){boundFn=false;firingFn=false;}
if(!boundFn){if(isLateBinding){boundFn=scope[fn];if(!boundFn){continue;}}
else{boundFn=fn;}
listener.boundFn=boundFn;}
if(!firingFn){firingFn=boundFn;if(options.buffer){firingFn=Ext.Function.createBuffered(firingFn,options.buffer,scope);}
if(options.delay){firingFn=Ext.Function.createDelayed(firingFn,options.delay,scope);}
listener.firingFn=firingFn;}
firingArguments[optionsArgumentIndex]=options;args=firingArguments;if(options.args){args=options.args.concat(args);}
if(options.single===true){listener.stack.remove(fn,scope,listener.order);}
result=firingFn.apply(scope,args);if(result===false){this.stop();}
if(this.isStopped){break;}
if(this.isPausing){this.isPaused=true;firingListeners.splice(0,i+1);return;}}
this.isFiring=false;this.listenerStacks=null;firingListeners.length=0;firingArguments.length=0;this.connectingController=null;},connect:function(controller){this.connectingController=controller;},resume:function(){var connectingController=this.connectingController;this.isPausing=false;if(this.isPaused&&this.firingListeners.length>0){this.isPaused=false;this.doFire();}
if(connectingController){connectingController.resume();}
return this;},isInterrupted:function(){return this.isStopped||this.isPaused;},stop:function(){var connectingController=this.connectingController;this.isStopped=true;if(connectingController){this.connectingController=null;connectingController.stop();}
this.isFiring=false;this.listenerStacks=null;return this;},pause:function(){var connectingController=this.connectingController;this.isPausing=true;if(connectingController){connectingController.pause();}
return this;}});Ext.define('Ext.event.publisher.Publisher',{targetType:'',idSelectorRegex:/^#([\w\-]+)$/i,constructor:function(){var handledEvents=this.handledEvents,handledEventsMap,i,ln,event;handledEventsMap=this.handledEventsMap={};for(i=0,ln=handledEvents.length;i<ln;i++){event=handledEvents[i];handledEventsMap[event]=true;}
this.subscribers={};return this;},handles:function(eventName){var map=this.handledEventsMap;return!!map[eventName]||!!map['*']||eventName==='*';},getHandledEvents:function(){return this.handledEvents;},setDispatcher:function(dispatcher){this.dispatcher=dispatcher;},subscribe:function(){return false;},unsubscribe:function(){return false;},unsubscribeAll:function(){delete this.subscribers;this.subscribers={};return this;},notify:function(){return false;},getTargetType:function(){return this.targetType;},dispatch:function(target,eventName,args){this.dispatcher.doDispatchEvent(this.targetType,target,eventName,args);}});Ext.define('Ext.event.Event',{alternateClassName:'Ext.EventObject',isStopped:false,set:function(name,value){if(arguments.length===1&&typeof name!='string'){var info=name;for(name in info){if(info.hasOwnProperty(name)){this[name]=info[name];}}}
else{this[name]=info[name];}},stopEvent:function(){return this.stopPropagation();},stopPropagation:function(){this.isStopped=true;return this;}});Ext.define('Ext.util.Point',{radianToDegreeConstant:180/Math.PI,statics:{fromEvent:function(e){var changedTouches=e.changedTouches,touch=(changedTouches&&changedTouches.length>0)?changedTouches[0]:e;return this.fromTouch(touch);},fromTouch:function(touch){return new this(touch.pageX,touch.pageY);},from:function(object){if(!object){return new this(0,0);}
if(!(object instanceof this)){return new this(object.x,object.y);}
return object;}},constructor:function(x,y){if(typeof x=='undefined'){x=0;}
if(typeof y=='undefined'){y=0;}
this.x=x;this.y=y;return this;},clone:function(){return new this.self(this.x,this.y);},copy:function(){return this.clone.apply(this,arguments);},copyFrom:function(point){this.x=point.x;this.y=point.y;return this;},toString:function(){return"Point["+this.x+","+this.y+"]";},equals:function(point){return(this.x===point.x&&this.y===point.y);},isCloseTo:function(point,threshold){if(typeof threshold=='number'){threshold={x:threshold};threshold.y=threshold.x;}
var x=point.x,y=point.y,thresholdX=threshold.x,thresholdY=threshold.y;return(this.x<=x+thresholdX&&this.x>=x-thresholdX&&this.y<=y+thresholdY&&this.y>=y-thresholdY);},isWithin:function(){return this.isCloseTo.apply(this,arguments);},translate:function(x,y){this.x+=x;this.y+=y;return this;},roundedEquals:function(point){return(Math.round(this.x)===Math.round(point.x)&&Math.round(this.y)===Math.round(point.y));},getDistanceTo:function(point){var deltaX=this.x-point.x,deltaY=this.y-point.y;return Math.sqrt(deltaX*deltaX+deltaY*deltaY);},getAngleTo:function(point){var deltaX=this.x-point.x,deltaY=this.y-point.y;return Math.atan2(deltaY,deltaX)*this.radianToDegreeConstant;}});Ext.define('Ext.ComponentManager',{alternateClassName:'Ext.ComponentMgr',singleton:true,constructor:function(){var map={};this.all={map:map,getArray:function(){var list=[],id;for(id in map){list.push(map[id]);}
return list;}};this.map=map;},register:function(component){var id=component.getId();if(this.map[id]){Ext.Logger.warn('Registering a component with a id (`'+id+'`) which has already been used. Please ensure the existing component has been destroyed (`Ext.Component#destroy()`.');}
this.map[component.getId()]=component;},unregister:function(component){delete this.map[component.getId()];},isRegistered:function(component){return this.map[component]!==undefined;},get:function(id){return this.map[id];},create:function(component,defaultType){if(component.isComponent){return component;}
else if(Ext.isString(component)){return Ext.createByAlias('widget.'+component);}
else{var type=component.xtype||defaultType;return Ext.createByAlias('widget.'+type,component);}},registerType:Ext.emptyFn});Ext.define('Ext.behavior.Behavior',{constructor:function(component){this.component=component;component.on('destroy','onComponentDestroy',this);},onComponentDestroy:Ext.emptyFn});Ext.define('Ext.fx.State',{isAnimatable:{'background-color':true,'background-image':true,'background-position':true,'border-bottom-color':true,'border-bottom-width':true,'border-color':true,'border-left-color':true,'border-left-width':true,'border-right-color':true,'border-right-width':true,'border-spacing':true,'border-top-color':true,'border-top-width':true,'border-width':true,'bottom':true,'color':true,'crop':true,'font-size':true,'font-weight':true,'height':true,'left':true,'letter-spacing':true,'line-height':true,'margin-bottom':true,'margin-left':true,'margin-right':true,'margin-top':true,'max-height':true,'max-width':true,'min-height':true,'min-width':true,'opacity':true,'outline-color':true,'outline-offset':true,'outline-width':true,'padding-bottom':true,'padding-left':true,'padding-right':true,'padding-top':true,'right':true,'text-indent':true,'text-shadow':true,'top':true,'vertical-align':true,'visibility':true,'width':true,'word-spacing':true,'z-index':true,'zoom':true,'transform':true},constructor:function(data){this.data={};this.set(data);},setConfig:function(data){this.set(data);return this;},setRaw:function(data){this.data=data;return this;},clear:function(){return this.setRaw({});},setTransform:function(name,value){var data=this.data,isArray=Ext.isArray(value),transform=data.transform,ln,key;if(!transform){transform=data.transform={translateX:0,translateY:0,translateZ:0,scaleX:1,scaleY:1,scaleZ:1,rotate:0,rotateX:0,rotateY:0,rotateZ:0,skewX:0,skewY:0};}
if(typeof name=='string'){switch(name){case'translate':if(isArray){ln=value.length;if(ln==0){break;}
transform.translateX=value[0];if(ln==1){break;}
transform.translateY=value[1];if(ln==2){break;}
transform.translateZ=value[2];}
else{transform.translateX=value;}
break;case'rotate':if(isArray){ln=value.length;if(ln==0){break;}
transform.rotateX=value[0];if(ln==1){break;}
transform.rotateY=value[1];if(ln==2){break;}
transform.rotateZ=value[2];}
else{transform.rotate=value;}
break;case'scale':if(isArray){ln=value.length;if(ln==0){break;}
transform.scaleX=value[0];if(ln==1){break;}
transform.scaleY=value[1];if(ln==2){break;}
transform.scaleZ=value[2];}
else{transform.scaleX=value;transform.scaleY=value;}
break;case'skew':if(isArray){ln=value.length;if(ln==0){break;}
transform.skewX=value[0];if(ln==1){break;}
transform.skewY=value[1];}
else{transform.skewX=value;}
break;default:transform[name]=value;}}
else{for(key in name){if(name.hasOwnProperty(key)){value=name[key];this.setTransform(key,value);}}}},set:function(name,value){var data=this.data,key;if(typeof name!='string'){for(key in name){value=name[key];if(key==='transform'){this.setTransform(value);}
else{data[key]=value;}}}
else{if(name==='transform'){this.setTransform(value);}
else{data[name]=value;}}
return this;},unset:function(name){var data=this.data;if(data.hasOwnProperty(name)){delete data[name];}
return this;},getData:function(){return this.data;}});Ext.define('Ext.mixin.Mixin',{onClassExtended:function(cls,data){var mixinConfig=data.mixinConfig,parentClassMixinConfig,beforeHooks,afterHooks;if(mixinConfig){parentClassMixinConfig=cls.superclass.mixinConfig;if(parentClassMixinConfig){mixinConfig=data.mixinConfig=Ext.merge({},parentClassMixinConfig,mixinConfig);}
data.mixinId=mixinConfig.id;beforeHooks=mixinConfig.beforeHooks;afterHooks=mixinConfig.hooks||mixinConfig.afterHooks;if(beforeHooks||afterHooks){Ext.Function.interceptBefore(data,'onClassMixedIn',function(targetClass){var mixin=this.prototype;if(beforeHooks){Ext.Object.each(beforeHooks,function(from,to){targetClass.override(to,function(){if(mixin[from].apply(this,arguments)!==false){return this.callOverridden(arguments);}});});}
if(afterHooks){Ext.Object.each(afterHooks,function(from,to){targetClass.override(to,function(){var ret=this.callOverridden(arguments);mixin[from].apply(this,arguments);return ret;});});}});}}}});Ext.define('Ext.XTemplateParser',{constructor:function(config){Ext.apply(this,config);},doTpl:Ext.emptyFn,parse:function(str){var me=this,len=str.length,aliases={elseif:'elif'},topRe=me.topRe,actionsRe=me.actionsRe,index,stack,s,m,t,prev,frame,subMatch,begin,end,actions;me.level=0;me.stack=stack=[];for(index=0;index<len;index=end){topRe.lastIndex=index;m=topRe.exec(str);if(!m){me.doText(str.substring(index,len));break;}
begin=m.index;end=topRe.lastIndex;if(index<begin){me.doText(str.substring(index,begin));}
if(m[1]){end=str.indexOf('%}',begin+2);me.doEval(str.substring(begin+2,end));end+=2;}else if(m[2]){end=str.indexOf(']}',begin+2);me.doExpr(str.substring(begin+2,end));end+=2;}else if(m[3]){me.doTag(m[3]);}else if(m[4]){actions=null;while((subMatch=actionsRe.exec(m[4]))!==null){s=subMatch[2]||subMatch[3];if(s){s=Ext.String.htmlDecode(s);t=subMatch[1];t=aliases[t]||t;actions=actions||{};prev=actions[t];if(typeof prev=='string'){actions[t]=[prev,s];}else if(prev){actions[t].push(s);}else{actions[t]=s;}}}
if(!actions){if(me.elseRe.test(m[4])){me.doElse();}else if(me.defaultRe.test(m[4])){me.doDefault();}else{me.doTpl();stack.push({type:'tpl'});}}
else if(actions['if']){me.doIf(actions['if'],actions)
stack.push({type:'if'});}
else if(actions['switch']){me.doSwitch(actions['switch'],actions)
stack.push({type:'switch'});}
else if(actions['case']){me.doCase(actions['case'],actions);}
else if(actions['elif']){me.doElseIf(actions['elif'],actions);}
else if(actions['for']){++me.level;me.doFor(actions['for'],actions);stack.push({type:'for',actions:actions});}
else if(actions.exec){me.doExec(actions.exec,actions);stack.push({type:'exec',actions:actions});}}else{frame=stack.pop();me.doEnd(frame.type,frame.actions);if(frame.type=='for'){--me.level;}}}},topRe:/(?:(\{\%)|(\{\[)|\{([^{}]*)\})|(?:<tpl([^>]*)\>)|(?:<\/tpl>)/g,actionsRe:/\s*(elif|elseif|if|for|exec|switch|case|eval)\s*\=\s*(?:(?:["]([^"]*)["])|(?:[']([^']*)[']))\s*/g,defaultRe:/^\s*default\s*$/,elseRe:/^\s*else\s*$/});Ext.define('Ext.util.Filter',{isFilter:true,config:{property:null,value:null,filterFn:Ext.emptyFn,anyMatch:false,exactMatch:false,caseSensitive:false,root:null,id:undefined,scope:null},applyId:function(id){if(!id){if(this.getProperty()){id=this.getProperty()+'-'+String(this.getValue());}
if(!id){id=Ext.id(null,'ext-filter-');}}
return id;},constructor:function(config){this.initConfig(config);},applyFilterFn:function(filterFn){if(filterFn===Ext.emptyFn){filterFn=this.getInitialConfig('filter');if(filterFn){return filterFn;}
var value=this.getValue();if(!this.getProperty()&&!value&&value!==0){Ext.Logger.error('A Filter requires either a property and value, or a filterFn to be set');return Ext.emptyFn;}
else{return this.createFilterFn();}}
return filterFn;},createFilterFn:function(){var me=this,matcher=me.createValueMatcher();return function(item){var root=me.getRoot(),property=me.getProperty();if(root){item=item[root];}
return matcher.test(item[property]);};},createValueMatcher:function(){var me=this,value=me.getValue(),anyMatch=me.getAnyMatch(),exactMatch=me.getExactMatch(),caseSensitive=me.getCaseSensitive(),escapeRe=Ext.String.escapeRegex;if(value===null||value===undefined||!value.exec){value=String(value);if(anyMatch===true){value=escapeRe(value);}else{value='^'+escapeRe(value);if(exactMatch===true){value+='$';}}
value=new RegExp(value,caseSensitive?'':'i');}
return value;}});Ext.define('Ext.util.Sorter',{isSorter:true,config:{property:null,sorterFn:null,root:null,transform:null,direction:"ASC",id:undefined},constructor:function(config){this.initConfig(config);},applySorterFn:function(sorterFn){if(!sorterFn&&!this.getProperty()){Ext.Logger.error("A Sorter requires either a property or a sorterFn.");}
return sorterFn;},applyProperty:function(property){if(!property&&!this.getSorterFn()){Ext.Logger.error("A Sorter requires either a property or a sorterFn.");}
return property;},applyId:function(id){if(!id){id=this.getProperty();if(!id){id=Ext.id(null,'ext-sorter-');}}
return id;},createSortFunction:function(sorterFn){var me=this,modifier=me.getDirection().toUpperCase()=="DESC"?-1:1;return function(o1,o2){return modifier*sorterFn.call(me,o1,o2);};},defaultSortFn:function(item1,item2){var me=this,transform=me._transform,root=me._root,value1,value2,property=me._property;if(root!==null){item1=item1[root];item2=item2[root];}
value1=item1[property];value2=item2[property];if(transform){value1=transform(value1);value2=transform(value2);}
return value1>value2?1:(value1<value2?-1:0);},updateDirection:function(){this.updateSortFn();},updateSortFn:function(){this.sort=this.createSortFunction(this.getSorterFn()||this.defaultSortFn);},toggle:function(){this.setDirection(Ext.String.toggle(this.getDirection(),"ASC","DESC"));}});Ext.define('Ext.fx.easing.Abstract',{config:{startTime:0,startValue:0},isEasing:true,isEnded:false,constructor:function(config){this.initConfig(config);return this;},applyStartTime:function(startTime){if(!startTime){startTime=Ext.Date.now();}
return startTime;},updateStartTime:function(startTime){this.reset();},reset:function(){this.isEnded=false;},getValue:Ext.emptyFn});Ext.define('Ext.fx.easing.Momentum',{extend:'Ext.fx.easing.Abstract',config:{acceleration:30,friction:0,startVelocity:0},alpha:0,updateFriction:function(friction){var theta=Math.log(1-(friction/10));this.theta=theta;this.alpha=theta/this.getAcceleration();},updateStartVelocity:function(velocity){this.velocity=velocity*this.getAcceleration();},updateAcceleration:function(acceleration){this.velocity=this.getStartVelocity()*acceleration;this.alpha=this.theta/acceleration;},getValue:function(){return this.getStartValue()-this.velocity*(1-this.getFrictionFactor())/this.theta;},getFrictionFactor:function(){var deltaTime=Ext.Date.now()-this.getStartTime();return Math.exp(deltaTime*this.alpha);},getVelocity:function(){return this.getFrictionFactor()*this.velocity;}});Ext.define('Ext.fx.easing.Bounce',{extend:'Ext.fx.easing.Abstract',config:{springTension:0.3,acceleration:30,startVelocity:0},getValue:function(){var deltaTime=Ext.Date.now()-this.getStartTime(),theta=(deltaTime/this.getAcceleration()),powTime=theta*Math.pow(Math.E,-this.getSpringTension()*theta);return this.getStartValue()+(this.getStartVelocity()*powTime);}});Ext.define('Ext.event.Dispatcher',{requires:['Ext.event.ListenerStack','Ext.event.Controller'],statics:{getInstance:function(){if(!this.instance){this.instance=new this();}
return this.instance;},setInstance:function(instance){this.instance=instance;return this;}},config:{publishers:{}},wildcard:'*',constructor:function(config){this.listenerStacks={};this.activePublishers={};this.publishersCache={};this.noActivePublishers=[];this.controller=null;this.initConfig(config);return this;},getListenerStack:function(targetType,target,eventName,createIfNotExist){var listenerStacks=this.listenerStacks,map=listenerStacks[targetType],listenerStack;createIfNotExist=Boolean(createIfNotExist);if(!map){if(createIfNotExist){listenerStacks[targetType]=map={};}
else{return null;}}
map=map[target];if(!map){if(createIfNotExist){listenerStacks[targetType][target]=map={};}
else{return null;}}
listenerStack=map[eventName];if(!listenerStack){if(createIfNotExist){map[eventName]=listenerStack=new Ext.event.ListenerStack();}
else{return null;}}
return listenerStack;},getController:function(targetType,target,eventName,connectedController){var controller=this.controller,info={targetType:targetType,target:target,eventName:eventName};if(!controller){this.controller=controller=new Ext.event.Controller();}
if(controller.isFiring){controller=new Ext.event.Controller();}
controller.setInfo(info);if(connectedController&&controller!==connectedController){controller.connect(connectedController);}
return controller;},applyPublishers:function(publishers){var i,publisher;this.publishersCache={};for(i in publishers){if(publishers.hasOwnProperty(i)){publisher=publishers[i];this.registerPublisher(publisher);}}
return publishers;},registerPublisher:function(publisher){var activePublishers=this.activePublishers,targetType=publisher.getTargetType(),publishers=activePublishers[targetType];if(!publishers){activePublishers[targetType]=publishers=[];}
publishers.push(publisher);publisher.setDispatcher(this);return this;},getCachedActivePublishers:function(targetType,eventName){var cache=this.publishersCache,publishers;if((publishers=cache[targetType])&&(publishers=publishers[eventName])){return publishers;}
return null;},cacheActivePublishers:function(targetType,eventName,publishers){var cache=this.publishersCache;if(!cache[targetType]){cache[targetType]={};}
cache[targetType][eventName]=publishers;return publishers;},getActivePublishers:function(targetType,eventName){var publishers,activePublishers,i,ln,publisher;if((publishers=this.getCachedActivePublishers(targetType,eventName))){return publishers;}
activePublishers=this.activePublishers[targetType];if(activePublishers){publishers=[];for(i=0,ln=activePublishers.length;i<ln;i++){publisher=activePublishers[i];if(publisher.handles(eventName)){publishers.push(publisher);}}}
else{publishers=this.noActivePublishers;}
return this.cacheActivePublishers(targetType,eventName,publishers);},hasListener:function(targetType,target,eventName){var listenerStack=this.getListenerStack(targetType,target,eventName);if(listenerStack){return listenerStack.count()>0;}
return false;},addListener:function(targetType,target,eventName){var publishers=this.getActivePublishers(targetType,eventName),ln=publishers.length,i;if(ln>0){for(i=0;i<ln;i++){publishers[i].subscribe(target,eventName);}}
return this.doAddListener.apply(this,arguments);},doAddListener:function(targetType,target,eventName,fn,scope,options,order){var listenerStack=this.getListenerStack(targetType,target,eventName,true);return listenerStack.add(fn,scope,options,order);},removeListener:function(targetType,target,eventName){var publishers=this.getActivePublishers(targetType,eventName),ln=publishers.length,i;if(ln>0){for(i=0;i<ln;i++){publishers[i].unsubscribe(target,eventName);}}
return this.doRemoveListener.apply(this,arguments);},doRemoveListener:function(targetType,target,eventName,fn,scope,order){var listenerStack=this.getListenerStack(targetType,target,eventName);if(listenerStack===null){return false;}
return listenerStack.remove(fn,scope,order);},clearListeners:function(targetType,target,eventName){var listenerStacks=this.listenerStacks,ln=arguments.length,stacks,publishers,i,publisherGroup;if(ln===3){if(listenerStacks[targetType]&&listenerStacks[targetType][target]){this.removeListener(targetType,target,eventName);delete listenerStacks[targetType][target][eventName];}}
else if(ln===2){if(listenerStacks[targetType]){stacks=listenerStacks[targetType][target];if(stacks){for(eventName in stacks){if(stacks.hasOwnProperty(eventName)){publishers=this.getActivePublishers(targetType,eventName);for(i=0,ln=publishers.length;i<ln;i++){publishers[i].unsubscribe(target,eventName,true);}}}
delete listenerStacks[targetType][target];}}}
else if(ln===1){publishers=this.activePublishers[targetType];for(i=0,ln=publishers.length;i<ln;i++){publishers[i].unsubscribeAll();}
delete listenerStacks[targetType];}
else{publishers=this.activePublishers;for(targetType in publishers){if(publishers.hasOwnProperty(targetType)){publisherGroup=publishers[targetType];for(i=0,ln=publisherGroup.length;i<ln;i++){publisherGroup[i].unsubscribeAll();}}}
delete this.listenerStacks;this.listenerStacks={};}
return this;},dispatchEvent:function(targetType,target,eventName){var publishers=this.getActivePublishers(targetType,eventName),ln=publishers.length,i;if(ln>0){for(i=0;i<ln;i++){publishers[i].notify(target,eventName);}}
return this.doDispatchEvent.apply(this,arguments);},doDispatchEvent:function(targetType,target,eventName,args,action,connectedController){var listenerStack=this.getListenerStack(targetType,target,eventName),wildcardStacks=this.getWildcardListenerStacks(targetType,target,eventName),controller;if((listenerStack===null||listenerStack.length==0)){if(wildcardStacks.length==0&&!action){return;}}
else{wildcardStacks.push(listenerStack);}
controller=this.getController(targetType,target,eventName,connectedController);controller.setListenerStacks(wildcardStacks);controller.fire(args,action);return!controller.isInterrupted();},getWildcardListenerStacks:function(targetType,target,eventName){var stacks=[],wildcard=this.wildcard,isEventNameNotWildcard=eventName!==wildcard,isTargetNotWildcard=target!==wildcard,stack;if(isEventNameNotWildcard&&(stack=this.getListenerStack(targetType,target,wildcard))){stacks.push(stack);}
if(isTargetNotWildcard&&(stack=this.getListenerStack(targetType,wildcard,eventName))){stacks.push(stack);}
return stacks;}});Ext.define('Ext.event.Dom',{extend:'Ext.event.Event',constructor:function(event){var target=event.target,touches;if(target&&target.nodeType!==1){target=target.parentNode;}
touches=event.changedTouches;if(touches){touches=touches[0];this.pageX=touches.pageX;this.pageY=touches.pageY;}
else{this.pageX=event.pageX;this.pageY=event.pageY;}
this.browserEvent=this.event=event;this.target=this.delegatedTarget=target;this.type=event.type;this.timeStamp=this.time=event.timeStamp;if(typeof this.time!='number'){this.time=new Date(this.time).getTime();}
return this;},stopEvent:function(){this.preventDefault();return this.callParent();},preventDefault:function(){this.browserEvent.preventDefault();},getPageX:function(){return this.browserEvent.pageX;},getPageY:function(){return this.browserEvent.pageY;},getXY:function(){if(!this.xy){this.xy=[this.getPageX(),this.getPageY()];}
return this.xy;},getTarget:function(selector,maxDepth,returnEl){if(arguments.length===0){return this.delegatedTarget;}
return selector?Ext.fly(this.target).findParent(selector,maxDepth,returnEl):(returnEl?Ext.get(this.target):this.target);},getTime:function(){return this.time;},setDelegatedTarget:function(target){this.delegatedTarget=target;},makeUnpreventable:function(){this.browserEvent.preventDefault=Ext.emptyFn;}});Ext.define('Ext.event.publisher.Dom',{extend:'Ext.event.publisher.Publisher',requires:['Ext.env.Browser','Ext.Element','Ext.event.Dom'],targetType:'element',idOrClassSelectorRegex:/^([#|\.])([\w\-]+)$/,handledEvents:['click','focus','blur','mousemove','mousedown','mouseup','mouseover','mouseout','keyup','keydown','keypress','submit','transitionend','animationstart','animationend'],classNameSplitRegex:/\s+/,SELECTOR_ALL:'*',constructor:function(){var eventNames=this.getHandledEvents(),eventNameMap={},i,ln,eventName,vendorEventName;this.doBubbleEventsMap={'click':true,'submit':true,'mousedown':true,'mousemove':true,'mouseup':true,'mouseover':true,'mouseout':true,'transitionend':true};this.onEvent=Ext.Function.bind(this.onEvent,this);for(i=0,ln=eventNames.length;i<ln;i++){eventName=eventNames[i];vendorEventName=this.getVendorEventName(eventName);eventNameMap[vendorEventName]=eventName;this.attachListener(vendorEventName);}
this.eventNameMap=eventNameMap;return this.callParent();},getSubscribers:function(eventName){var subscribers=this.subscribers,eventSubscribers=subscribers[eventName];if(!eventSubscribers){eventSubscribers=subscribers[eventName]={id:{$length:0},className:{$length:0},selector:[],all:0,$length:0}}
return eventSubscribers;},getVendorEventName:function(eventName){if(eventName==='transitionend'){eventName=Ext.browser.getVendorProperyName('transitionEnd');}
else if(eventName==='animationstart'){eventName=Ext.browser.getVendorProperyName('animationStart');}
else if(eventName==='animationend'){eventName=Ext.browser.getVendorProperyName('animationEnd');}
return eventName;},attachListener:function(eventName){document.addEventListener(eventName,this.onEvent,!this.doesEventBubble(eventName));return this;},removeListener:function(eventName){document.removeEventListener(eventName,this.onEvent,!this.doesEventBubble(eventName));return this;},doesEventBubble:function(eventName){return!!this.doBubbleEventsMap[eventName];},subscribe:function(target,eventName){if(!this.handles(eventName)){return false;}
var idOrClassSelectorMatch=target.match(this.idOrClassSelectorRegex),subscribers=this.getSubscribers(eventName),idSubscribers=subscribers.id,classNameSubscribers=subscribers.className,selectorSubscribers=subscribers.selector,type,value;if(idOrClassSelectorMatch!==null){type=idOrClassSelectorMatch[1];value=idOrClassSelectorMatch[2];if(type==='#'){if(idSubscribers.hasOwnProperty(value)){idSubscribers[value]++;return true;}
idSubscribers[value]=1;idSubscribers.$length++;}
else{if(classNameSubscribers.hasOwnProperty(value)){classNameSubscribers[value]++;return true;}
classNameSubscribers[value]=1;classNameSubscribers.$length++;}}
else{if(target===this.SELECTOR_ALL){subscribers.all++;}
else{if(selectorSubscribers.hasOwnProperty(target)){selectorSubscribers[target]++;return true;}
selectorSubscribers[target]=1;selectorSubscribers.push(target);}}
subscribers.$length++;return true;},unsubscribe:function(target,eventName,all){if(!this.handles(eventName)){return false;}
var idOrClassSelectorMatch=target.match(this.idOrClassSelectorRegex),subscribers=this.getSubscribers(eventName),idSubscribers=subscribers.id,classNameSubscribers=subscribers.className,selectorSubscribers=subscribers.selector,type,value;all=Boolean(all);if(idOrClassSelectorMatch!==null){type=idOrClassSelectorMatch[1];value=idOrClassSelectorMatch[2];if(type==='#'){if(!idSubscribers.hasOwnProperty(value)||(!all&&--idSubscribers[value]>0)){return true;}
delete idSubscribers[value];idSubscribers.$length--;}
else{if(!classNameSubscribers.hasOwnProperty(value)||(!all&&--classNameSubscribers[value]>0)){return true;}
delete classNameSubscribers[value];classNameSubscribers.$length--;}}
else{if(target===this.SELECTOR_ALL){if(all){subscribers.all=0;}
else{subscribers.all--;}}
else{if(!selectorSubscribers.hasOwnProperty(target)||(!all&&--selectorSubscribers[target]>0)){return true;}
delete selectorSubscribers[target];Ext.Array.remove(selectorSubscribers,target);}}
subscribers.$length--;return true;},getElementTarget:function(target){if(target.nodeType!==1){target=target.parentNode;if(!target||target.nodeType!==1){return null;}}
return target;},getBubblingTargets:function(target){var targets=[];if(!target){return targets;}
do{targets[targets.length]=target;target=target.parentNode;}while(target&&target.nodeType===1);return targets;},dispatch:function(target,eventName,args){args.push(args[0].target);this.callParent(arguments);},publish:function(eventName,targets,event){var subscribers=this.getSubscribers(eventName),wildcardSubscribers;if(subscribers.$length===0||!this.doPublish(subscribers,eventName,targets,event)){wildcardSubscribers=this.getSubscribers('*');if(wildcardSubscribers.$length>0){this.doPublish(wildcardSubscribers,eventName,targets,event);}}
return this;},doPublish:function(subscribers,eventName,targets,event){var idSubscribers=subscribers.id,classNameSubscribers=subscribers.className,selectorSubscribers=subscribers.selector,hasIdSubscribers=idSubscribers.$length>0,hasClassNameSubscribers=classNameSubscribers.$length>0,hasSelectorSubscribers=selectorSubscribers.length>0,hasAllSubscribers=subscribers.all>0,isClassNameHandled={},args=[event],hasDispatched=false,classNameSplitRegex=this.classNameSplitRegex,i,ln,j,subLn,target,id,className,classNames,selector;for(i=0,ln=targets.length;i<ln;i++){target=targets[i];event.setDelegatedTarget(target);if(hasIdSubscribers){id=target.id;if(id){if(idSubscribers.hasOwnProperty(id)){hasDispatched=true;this.dispatch('#'+id,eventName,args);}}}
if(hasClassNameSubscribers){className=target.className;if(className){classNames=className.split(classNameSplitRegex);for(j=0,subLn=classNames.length;j<subLn;j++){className=classNames[j];if(!isClassNameHandled[className]){isClassNameHandled[className]=true;if(classNameSubscribers.hasOwnProperty(className)){hasDispatched=true;this.dispatch('.'+className,eventName,args);}}}}}
if(event.isStopped){return hasDispatched;}}
if(hasAllSubscribers&&!hasDispatched){event.setDelegatedTarget(event.browserEvent.target);hasDispatched=true;this.dispatch(this.SELECTOR_ALL,eventName,args);if(event.isStopped){return hasDispatched;}}
if(hasSelectorSubscribers){for(j=0,subLn=targets.length;j<subLn;j++){target=targets[j];for(i=0,ln=selectorSubscribers.length;i<ln;i++){selector=selectorSubscribers[i];if(this.matchesSelector(target,selector)){event.setDelegatedTarget(target);hasDispatched=true;this.dispatch(selector,eventName,args);}
if(event.isStopped){return hasDispatched;}}}}
return hasDispatched;},matchesSelector:function(element,selector){if('webkitMatchesSelector'in element){return element.webkitMatchesSelector(selector);}
return Ext.DomQuery.is(element,selector);},onEvent:function(e){var eventName=this.eventNameMap[e.type];if(!eventName||this.getSubscribersCount(eventName)===0){return;}
var target=this.getElementTarget(e.target),targets;if(!target){return;}
if(this.doesEventBubble(eventName)){targets=this.getBubblingTargets(target);}
else{targets=[target];}
this.publish(eventName,targets,new Ext.event.Dom(e));},hasSubscriber:function(target,eventName){if(!this.handles(eventName)){return false;}
var match=target.match(this.idOrClassSelectorRegex),subscribers=this.getSubscribers(eventName),type,value;if(match!==null){type=match[1];value=match[2];if(type==='#'){return subscribers.id.hasOwnProperty(value);}
else{return subscribers.className.hasOwnProperty(value);}}
else{return(subscribers.selector.hasOwnProperty(target)&&Ext.Array.indexOf(subscribers.selector,target)!==-1);}
return false;},getSubscribersCount:function(eventName){if(!this.handles(eventName)){return 0;}
return this.getSubscribers(eventName).$length+this.getSubscribers('*').$length;}});Ext.define('Ext.util.LineSegment',{requires:['Ext.util.Point'],constructor:function(point1,point2){var Point=Ext.util.Point;this.point1=Point.from(point1);this.point2=Point.from(point2);},intersects:function(lineSegment){var point1=this.point1,point2=this.point2,point3=lineSegment.point1,point4=lineSegment.point2,x1=point1.x,x2=point2.x,x3=point3.x,x4=point4.x,y1=point1.y,y2=point2.y,y3=point3.y,y4=point4.y,d=(x1-x2)*(y3-y4)-(y1-y2)*(x3-x4),xi,yi;if(d==0){return null;}
xi=((x3-x4)*(x1*y2-y1*x2)-(x1-x2)*(x3*y4-y3*x4))/d;yi=((y3-y4)*(x1*y2-y1*x2)-(y1-y2)*(x3*y4-y3*x4))/d;if(xi<Math.min(x1,x2)||xi>Math.max(x1,x2)||xi<Math.min(x3,x4)||xi>Math.max(x3,x4)||yi<Math.min(y1,y2)||yi>Math.max(y1,y2)||yi<Math.min(y3,y4)||yi>Math.max(y3,y4)){return null;}
return new Ext.util.Point(xi,yi);},toString:function(){return this.point1.toString()+" "+this.point2.toString();}});Ext.define('Ext.mixin.Traversable',{extend:'Ext.mixin.Mixin',mixinConfig:{id:'traversable'},setParent:function(parent){this.parent=parent;return this;},hasParent:function(){return Boolean(this.parent);},getParent:function(){return this.parent;},getAncestors:function(){var ancestors=[],parent=this.getParent();while(parent){ancestors.push(parent);parent=parent.getParent();}
return ancestors;},getAncestorIds:function(){var ancestorIds=[],parent=this.getParent();while(parent){ancestorIds.push(parent.getId());parent=parent.getParent();}
return ancestorIds;}});Ext.define('Ext.XTemplateCompiler',{extend:'Ext.XTemplateParser',useEval:Ext.isGecko,useFormat:true,propNameRe:/^[\w\d\$]*$/,compile:function(tpl){var me=this,code=me.generate(tpl);return me.useEval?me.evalTpl(code):(new Function('Ext',code))(Ext);},generate:function(tpl){var me=this;me.body=['var c0=values, p0=parent, n0=xcount, i0=xindex;\n'];me.funcs=['var fm=Ext.util.Format;'];me.switches=[];me.parse(tpl);me.funcs.push((me.useEval?'$=':'return')+' function ('+me.fnArgs+') {',me.body.join(''),'}');var code=me.funcs.join('\n');return code;},doText:function(text){text=text.replace(this.aposRe,"\\'");text=text.replace(this.newLineRe,'\\n');this.body.push('out.push(\'',text,'\')\n');},doExpr:function(expr){this.body.push('out.push(String(',expr,'))\n');},doTag:function(tag){this.doExpr(this.parseTag(tag));},doElse:function(){this.body.push('} else {\n');},doEval:function(text){this.body.push(text,'\n');},doIf:function(action,actions){var me=this;if(me.propNameRe.test(action)){me.body.push('if (',me.parseTag(action),') {\n');}
else{me.body.push('if (',me.addFn(action),me.callFn,') {\n');}
if(actions.exec){me.doExec(actions.exec);}},doElseIf:function(action,actions){var me=this;if(me.propNameRe.test(action)){me.body.push('} else if (',me.parseTag(action),') {\n');}
else{me.body.push('} else if (',me.addFn(action),me.callFn,') {\n');}
if(actions.exec){me.doExec(actions.exec);}},doSwitch:function(action){var me=this;if(me.propNameRe.test(action)){me.body.push('switch (',me.parseTag(action),') {\n');}
else{me.body.push('switch (',me.addFn(action),me.callFn,') {\n');}
me.switches.push(0);},doCase:function(action){var me=this,cases=Ext.isArray(action)?action:[action],n=me.switches.length-1,match,i;if(me.switches[n]){me.body.push('break;\n');}else{me.switches[n]++;}
for(i=0,n=cases.length;i<n;++i){match=me.intRe.exec(cases[i]);cases[i]=match?match[1]:("'"+cases[i].replace(me.aposRe,"\\'")+"'");}
me.body.push('case ',cases.join(': case '),':\n');},doDefault:function(){var me=this,n=me.switches.length-1;if(me.switches[n]){me.body.push('break;\n');}else{me.switches[n]++;}
me.body.push('default:\n');},doEnd:function(type,actions){var me=this,L=me.level-1;if(type=='for'){if(actions.exec){me.doExec(actions.exec);}
me.body.push('}\n');me.body.push('parent=p',L,';values=r',L+1,';xcount=n',L,';xindex=i',L,'\n');}else if(type=='if'||type=='switch'){me.body.push('}\n');}},doFor:function(action,actions){var me=this,s=me.addFn(action),L=me.level,up=L-1;me.body.push('var c',L,'=',s,me.callFn,', a',L,'=Ext.isArray(c',L,'),p',L,'=(parent=c',up,'),r',L,'=values\n','for (var i',L,'=0,n',L,'=a',L,'?c',L,'.length:(c',L,'?1:0), xcount=n',L,';i',L,'<n'+L+';++i',L,'){\n','values=a',L,'?c',L,'[i',L,']:c',L,'\n','xindex=i',L,'+1\n');},doExec:function(action,actions){var me=this,name='f'+me.funcs.length;me.funcs.push('function '+name+'('+me.fnArgs+') {',' try { with(values) {','  '+action,' }} catch(e) {}','}');me.body.push(name+me.callFn+'\n');},addFn:function(body){var me=this,name='f'+me.funcs.length;if(body==='.'){me.funcs.push('function '+name+'('+me.fnArgs+') {',' return values','}');}else if(body==='..'){me.funcs.push('function '+name+'('+me.fnArgs+') {',' return parent','}');}else{me.funcs.push('function '+name+'('+me.fnArgs+') {',' try { with(values) {','  return('+body+')',' }} catch(e) {}','}');}
return name;},parseTag:function(tag){var m=this.tagRe.exec(tag),name=m[1],format=m[2],args=m[3],math=m[4],v;if(name=='.'){v='Ext.Array.indexOf(["string", "number", "boolean"], typeof values) > -1 || Ext.isDate(values) ? values : ""';}
else if(name=='#'){v='xindex';}
else if(name.substr(0,7)=="parent."){v=name;}
else if((name.indexOf('.')!==-1)&&(name.indexOf('-')===-1)){v="values."+name;}
else{v="values['"+name+"']";}
if(math){v='('+v+math+')';}
if(format&&this.useFormat){args=args?','+args:"";if(format.substr(0,5)!="this."){format="fm."+format+'(';}else{format+='(';}}else{args='';format="("+v+" === undefined ? '' : ";}
return format+v+args+')';},evalTpl:function($){eval($);return $;},newLineRe:/\r\n|\r|\n/g,aposRe:/[']/g,intRe:/^\s*(\d+)\s*$/,tagRe:/([\w-\.\#]+)(?:\:([\w\.]*)(?:\((.*?)?\))?)?(\s?[\+\-\*\/]\s?[\d\.\+\-\*\/\(\)]+)?/},function(){var proto=this.prototype;proto.fnArgs='out,values,parent,xindex,xcount';proto.callFn='.call(this,'+proto.fnArgs+')';});Ext.define("Ext.util.Sortable",{isSortable:true,defaultSortDirection:"ASC",requires:['Ext.util.Sorter'],initSortable:function(){var me=this,sorters=me.sorters;me.sorters=Ext.create('Ext.util.AbstractMixedCollection',false,function(item){return item.id||item.property;});if(sorters){me.sorters.addAll(me.decodeSorters(sorters));}},sort:function(sorters,direction,where,doSort){var me=this,sorter,sorterFn,newSorters;if(Ext.isArray(sorters)){doSort=where;where=direction;newSorters=sorters;}
else if(Ext.isObject(sorters)){doSort=where;where=direction;newSorters=[sorters];}
else if(Ext.isString(sorters)){sorter=me.sorters.get(sorters);if(!sorter){sorter={property:sorters,direction:direction};newSorters=[sorter];}
else if(direction===undefined){sorter.toggle();}
else{sorter.setDirection(direction);}}
if(newSorters&&newSorters.length){newSorters=me.decodeSorters(newSorters);if(Ext.isString(where)){if(where==='prepend'){sorters=me.sorters.clone().items;me.sorters.clear();me.sorters.addAll(newSorters);me.sorters.addAll(sorters);}
else{me.sorters.addAll(newSorters);}}
else{me.sorters.clear();me.sorters.addAll(newSorters);}
if(doSort!==false){me.onBeforeSort(newSorters);}}
if(doSort!==false){sorters=me.sorters.items;if(sorters.length){sorterFn=function(r1,r2){var result=sorters[0].sort(r1,r2),length=sorters.length,i;for(i=1;i<length;i++){result=result||sorters[i].sort.call(this,r1,r2);}
return result;};me.doSort(sorterFn);}}
return sorters;},onBeforeSort:Ext.emptyFn,decodeSorters:function(sorters){if(!Ext.isArray(sorters)){if(sorters===undefined){sorters=[];}else{sorters=[sorters];}}
var length=sorters.length,Sorter=Ext.util.Sorter,fields=this.model?this.model.prototype.fields:null,field,config,i;for(i=0;i<length;i++){config=sorters[i];if(!(config instanceof Sorter)){if(Ext.isString(config)){config={property:config};}
Ext.applyIf(config,{root:this.sortRoot,direction:"ASC"});if(config.fn){config.sorterFn=config.fn;}
if(typeof config=='function'){config={sorterFn:config};}
if(fields&&!config.transform){field=fields.get(config.property);config.transform=field?field.sortType:undefined;}
sorters[i]=Ext.create('Ext.util.Sorter',config);}}
return sorters;},getSorters:function(){return this.sorters.items;}});Ext.define('Ext.mixin.Observable',{requires:['Ext.event.Dispatcher'],extend:'Ext.mixin.Mixin',mixins:['Ext.mixin.Identifiable'],mixinConfig:{id:'observable',hooks:{destroy:'destroy'}},alternateClassName:'Ext.util.Observable',isObservable:true,observableType:'observable',validIdRegex:/^([\w\-]+)$/,observableIdPrefix:'#',listenerOptionsRegex:/^(?:delegate|single|delay|buffer|args|prepend)$/,config:{listeners:null,bubbleEvents:null},constructor:function(config){this.initConfig(config);},applyListeners:function(listeners){if(listeners){this.addListener(listeners);}},applyBubbleEvents:function(bubbleEvents){if(bubbleEvents){this.enableBubble(bubbleEvents);}},getOptimizedObservableId:function(){return this.observableId;},getObservableId:function(){if(!this.observableId){var id=this.getUniqueId();if(!id.match(this.validIdRegex)){Ext.Logger.error("Invalid unique id of '"+id+"' for this object",this);}
this.observableId=this.observableIdPrefix+id;this.getObservableId=this.getOptimizedObservableId;}
return this.observableId;},getOptimizedEventDispatcher:function(){return this.eventDispatcher;},getEventDispatcher:function(){if(!this.eventDispatcher){this.eventDispatcher=Ext.event.Dispatcher.getInstance();this.getEventDispatcher=this.getOptimizedEventDispatcher;this.getListeners();this.getBubbleEvents();}
return this.eventDispatcher;},getManagedListeners:function(object,eventName){var id=object.getUniqueId(),managedListeners=this.managedListeners;if(!managedListeners){this.managedListeners=managedListeners={};}
if(!managedListeners[id]){managedListeners[id]={};object.doAddListener('destroy','clearManagedListeners',this,{single:true,args:[object]});}
if(!managedListeners[id][eventName]){managedListeners[id][eventName]=[];}
return managedListeners[id][eventName];},getUsedSelectors:function(){var selectors=this.usedSelectors;if(!selectors){selectors=this.usedSelectors=[];selectors.$map={};}
return selectors;},fireEvent:function(eventName){var args=Array.prototype.slice.call(arguments,1);return this.doFireEvent(eventName,args);},fireAction:function(eventName,args,fn,scope,options,order){var fnType=typeof fn,action;if(args===undefined){args=[];}
if(fnType!='undefined'){action={fn:fn,isLateBinding:fnType=='string',scope:scope||this,options:options||{},order:order};}
return this.doFireEvent(eventName,args,action);},doFireEvent:function(eventName,args,action,connectedController){if(this.eventFiringSuspended){return;}
var id=this.getObservableId(),dispatcher=this.getEventDispatcher();return dispatcher.dispatchEvent(this.observableType,id,eventName,args,action,connectedController);},doAddListener:function(name,fn,scope,options,order){var isManaged=(scope&&scope!==this&&scope.isIdentifiable),usedSelectors=this.getUsedSelectors(),usedSelectorsMap=usedSelectors.$map,selector=this.getObservableId(),isAdded,managedListeners,delegate;if(!options){options={};}
if(!scope){scope=this;}
if(options.delegate){delegate=options.delegate;selector+=' '+delegate;}
if(!(selector in usedSelectorsMap)){usedSelectorsMap[selector]=true;usedSelectors.push(selector);}
isAdded=this.addDispatcherListener(selector,name,fn,scope,options,order);if(isAdded&&isManaged){managedListeners=this.getManagedListeners(scope,name);managedListeners.push({delegate:delegate,scope:scope,fn:fn,order:order});}
return isAdded;},addDispatcherListener:function(selector,name,fn,scope,options,order){return this.getEventDispatcher().addListener(this.observableType,selector,name,fn,scope,options,order);},doRemoveListener:function(name,fn,scope,options,order){var isManaged=(scope&&scope!==this&&scope.isIdentifiable),selector=this.getObservableId(),isRemoved,managedListeners,i,ln,listener,delegate;if(options&&options.delegate){delegate=options.delegate;selector+=' '+delegate;}
if(!scope){scope=this;}
isRemoved=this.removeDispatcherListener(selector,name,fn,scope,order);if(isRemoved&&isManaged){managedListeners=this.getManagedListeners(scope,name);for(i=0,ln=managedListeners.length;i<ln;i++){listener=managedListeners[i];if(listener.fn===fn&&listener.scope===scope&&listener.delegate===delegate&&listener.order===order){managedListeners.splice(i,1);break;}}}
return isRemoved;},removeDispatcherListener:function(selector,name,fn,scope,order){return this.getEventDispatcher().removeListener(this.observableType,selector,name,fn,scope,order);},clearManagedListeners:function(object){var managedListeners=this.managedListeners,id,namedListeners,listeners,eventName,i,ln,listener,options;if(!managedListeners){return this;}
if(object){if(typeof object!='string'){id=object.getUniqueId();}
else{id=object;}
namedListeners=managedListeners[id];for(eventName in namedListeners){if(namedListeners.hasOwnProperty(eventName)){listeners=namedListeners[eventName];for(i=0,ln=listeners.length;i<ln;i++){listener=listeners[i];options={};if(listener.delegate){options.delegate=listener.delegate;}
if(this.doRemoveListener(eventName,listener.fn,listener.scope,options,listener.order)){i--;ln--;}}}}
delete managedListeners[id];return this;}
for(id in managedListeners){if(managedListeners.hasOwnProperty(id)){this.clearManagedListeners(id);}}},changeListener:function(actionFn,eventName,fn,scope,options,order){var eventNames,listeners,listenerOptionsRegex,actualOptions,name,value,i,ln,listener,valueType;if(typeof fn!='undefined'){if(typeof eventName!='string'){for(i=0,ln=eventName.length;i<ln;i++){name=eventName[i];actionFn.call(this,name,fn,scope,options,order);}
return this;}
actionFn.call(this,eventName,fn,scope,options,order);}
else if(Ext.isArray(eventName)){listeners=eventName;for(i=0,ln=listeners.length;i<ln;i++){listener=listeners[i];actionFn.call(this,listener.event,listener.fn,listener.scope,listener,listener.order);}}
else{listenerOptionsRegex=this.listenerOptionsRegex;options=eventName;eventNames=[];listeners=[];actualOptions={};for(name in options){value=options[name];if(name==='scope'){scope=value;continue;}
else if(name==='order'){order=value;continue;}
if(!listenerOptionsRegex.test(name)){valueType=typeof value;if(valueType!='string'&&valueType!='function'){actionFn.call(this,name,value.fn,value.scope||scope,value,value.order||order);continue;}
eventNames.push(name);listeners.push(value);}
else{actualOptions[name]=value;}}
for(i=0,ln=eventNames.length;i<ln;i++){actionFn.call(this,eventNames[i],listeners[i],scope,actualOptions,order);}}},addListener:function(eventName,fn,scope,options,order){return this.changeListener(this.doAddListener,eventName,fn,scope,options,order);},addBeforeListener:function(eventName,fn,scope,options){return this.addListener(eventName,fn,scope,options,'before');},addAfterListener:function(eventName,fn,scope,options){return this.addListener(eventName,fn,scope,options,'after');},removeListener:function(eventName,fn,scope,options,order){return this.changeListener(this.doRemoveListener,eventName,fn,scope,options,order);},removeBeforeListener:function(eventName,fn,scope,options){return this.removeListener(eventName,fn,scope,options,'before');},removeAfterListener:function(eventName,fn,scope,options){return this.removeListener(eventName,fn,scope,options,'after');},clearListeners:function(){var usedSelectors=this.getUsedSelectors(),dispatcher=this.getEventDispatcher(),i,ln,selector;for(i=0,ln=usedSelectors.length;i<ln;i++){selector=usedSelectors[i];dispatcher.clearListeners(this.observableType,selector);}},hasListener:function(eventName){return this.getEventDispatcher().hasListener(this.observableType,this.getObservableId(),eventName);},suspendEvents:function(queueSuspended){this.eventFiringSuspended=true;},resumeEvents:function(){this.eventFiringSuspended=false;},relayEvents:function(object,events,prefix){var i,ln,oldName,newName;if(typeof prefix=='undefined'){prefix='';}
if(typeof events=='string'){events=[events];}
if(Ext.isArray(events)){for(i=0,ln=events.length;i<ln;i++){oldName=events[i];newName=prefix+oldName;object.addListener(oldName,this.createEventRelayer(newName),this);}}
else{for(oldName in events){if(events.hasOwnProperty(oldName)){newName=prefix+events[oldName];object.addListener(oldName,this.createEventRelayer(newName),this);}}}
return this;},relayEvent:function(args,fn,scope,options,order){var fnType=typeof fn,controller=args[args.length-1],eventName=controller.getInfo().eventName,action;args=Array.prototype.slice.call(args,0,-2);args[0]=this;if(fnType!='undefined'){action={fn:fn,scope:scope||this,options:options||{},order:order,isLateBinding:fnType=='string'};}
return this.doFireEvent(eventName,args,action,controller);},createEventRelayer:function(newName){return function(){return this.doFireEvent(newName,Array.prototype.slice.call(arguments,0,-2));}},enableBubble:function(events){var isBubblingEnabled=this.isBubblingEnabled,i,ln,name;if(!isBubblingEnabled){isBubblingEnabled=this.isBubblingEnabled={};}
if(typeof events=='string'){events=Ext.Array.clone(arguments);}
for(i=0,ln=events.length;i<ln;i++){name=events[i];if(!isBubblingEnabled[name]){isBubblingEnabled[name]=true;this.addListener(name,this.createEventBubbler(name),this);}}},createEventBubbler:function(name){return function doBubbleEvent(){var bubbleTarget=('getBubbleTarget'in this)?this.getBubbleTarget():null;if(bubbleTarget&&bubbleTarget!==this&&bubbleTarget.isObservable){bubbleTarget.fireAction(name,Array.prototype.slice.call(arguments,0,-2),doBubbleEvent,bubbleTarget,null,'after');}}},getBubbleTarget:function(){return false;},destroy:function(){if(this.observableId){this.fireEvent('destroy',this);this.clearListeners();this.clearManagedListeners();}},addEvents:Ext.emptyFn},function(){this.createAlias({on:'addListener',un:'removeListener',onBefore:'addBeforeListener',onAfter:'addAfterListener',unBefore:'removeBeforeListener',unAfter:'removeAfterListener'});Ext.deprecateClassMethod(this,'addEvents',function(){},"addEvents() is deprecated. It's no longer needed to add events before firing");Ext.deprecateClassMethod(this,'addManagedListener',function(object,eventName,fn,scope,options){return object.addListener(eventName,fn,scope,options);},"addManagedListener() / mon() is deprecated, simply use addListener() / on(). All listeners are now automatically managed where necessary.");Ext.deprecateClassMethod(this,'removeManagedListener',function(object,eventName,fn,scope){return object.removeListener(eventName,fn,scope);},"removeManagedListener() / mun() is deprecated, simply use removeListener() / un(). All listeners are now automatically managed where necessary.");this.createAlias({mon:'addManagedListener',mun:'removeManagedListener'});});Ext.define('Ext.Evented',{alternateClassName:'Ext.EventedBase',mixins:['Ext.mixin.Observable'],statics:{generateSetter:function(nameMap){var internalName=nameMap.internal,applyName=nameMap.apply,changeEventName=nameMap.changeEvent,doSetName=nameMap.doSet;return function(value){var initialized=this.initialized,oldValue=this[internalName],applier=this[applyName];if(applier){value=applier.call(this,value,oldValue);if(typeof value=='undefined'){return this;}}
if(value!==oldValue){if(initialized){this.fireAction(changeEventName,[this,value,oldValue],this.doSet,this,{nameMap:nameMap});}
else{this[internalName]=value;this[doSetName].call(this,value,oldValue);}}
return this;}}},initialized:false,constructor:function(config){this.initialConfig=config;this.initialize();},initialize:function(){this.initConfig(this.initialConfig);this.initialized=true;},doSet:function(me,value,oldValue,options){var nameMap=options.nameMap;me[nameMap.internal]=value;me[nameMap.doSet].call(this,value,oldValue);},onClassExtended:function(Class,data){if(!data.hasOwnProperty('eventedConfig')){return;}
var ExtClass=Ext.Class,config=data.config,eventedConfig=data.eventedConfig,name,nameMap;data.config=(config)?Ext.applyIf(config,eventedConfig):eventedConfig;for(name in eventedConfig){if(eventedConfig.hasOwnProperty(name)){nameMap=ExtClass.getConfigNameMap(name);data[nameMap.set]=this.generateSetter(nameMap);}}}});Ext.define('Ext.fx.animation.Abstract',{extend:'Ext.Evented',isAnimation:true,requires:['Ext.fx.State'],config:{name:'',element:null,before:null,from:{},to:{},after:null,states:{},duration:300,easing:'linear',iteration:1,direction:'normal',delay:0,onBeforeStart:null,onEnd:null,onBeforeEnd:null,scope:null,reverse:null,preserveEndState:false,replacePrevious:true},STATE_FROM:'0%',STATE_TO:'100%',DIRECTION_UP:'up',DIRECTION_DOWN:'down',DIRECTION_LEFT:'left',DIRECTION_RIGHT:'right',stateNameRegex:/^(?:[\d\.]+)%$/,constructor:function(){this.states={};this.callParent(arguments);return this;},applyElement:function(element){return Ext.get(element);},applyBefore:function(before,current){if(before){return Ext.factory(before,Ext.fx.State,current);}},applyAfter:function(after,current){if(after){return Ext.factory(after,Ext.fx.State,current);}},setFrom:function(from){return this.setState(this.STATE_FROM,from);},setTo:function(to){return this.setState(this.STATE_TO,to);},getFrom:function(){return this.getState(this.STATE_FROM);},getTo:function(){return this.getState(this.STATE_TO);},setStates:function(states){var validNameRegex=this.stateNameRegex,name;for(name in states){if(validNameRegex.test(name)){this.setState(name,states[name]);}}
return this;},getStates:function(){return this.states;},setState:function(name,state){var states=this.getStates(),stateInstance;stateInstance=Ext.factory(state,Ext.fx.State,states[name]);if(stateInstance){states[name]=stateInstance;}
else if(name===this.STATE_TO){Ext.Logger.error("Setting and invalid '100%' / 'to' state of: "+state);}
return this;},getState:function(name){return this.getStates()[name];},getData:function(){var states=this.getStates(),statesData={},before=this.getBefore(),after=this.getAfter(),from=states[this.STATE_FROM],to=states[this.STATE_TO],fromData=from.getData(),toData=to.getData(),data,name,state;for(name in states){if(states.hasOwnProperty(name)){state=states[name];data=state.getData();statesData[name]=data;}}
if(Ext.os.is.Android2){statesData['0.0001%']=fromData;}
return{before:before?before.getData():{},after:after?after.getData():{},states:statesData,from:fromData,to:toData,duration:this.getDuration(),iteration:this.getIteration(),direction:this.getDirection(),easing:this.getEasing(),delay:this.getDelay(),onEnd:this.getOnEnd(),onBeforeEnd:this.getOnBeforeEnd(),onBeforeStart:this.getOnBeforeStart(),scope:this.getScope(),preserveEndState:this.getPreserveEndState(),replacePrevious:this.getReplacePrevious()};}});Ext.define('Ext.fx.animation.Slide',{extend:'Ext.fx.animation.Abstract',alternateClassName:'Ext.fx.animation.SlideIn',alias:['animation.slide','animation.slideIn'],config:{direction:'left',out:false,offset:0,easing:'auto',containerBox:'auto',elementBox:'auto',isElementBoxFit:true,useCssTransform:true},reverseDirectionMap:{up:'down',down:'up',left:'right',right:'left'},applyEasing:function(easing){if(easing==='auto'){return'ease-'+((this.getOut())?'in':'out');}
return easing;},getContainerBox:function(){var box=this._containerBox;if(box==='auto'){box=this.getElement().getParent().getPageBox();}
return box;},getElementBox:function(){var box=this._elementBox;if(this.getIsElementBoxFit()){return this.getContainerBox();}
if(box==='auto'){box=this.getElement().getPageBox();}
return box;},getData:function(){var elementBox=this.getElementBox(),containerBox=this.getContainerBox(),box=elementBox?elementBox:containerBox,from=this.getFrom(),to=this.getTo(),out=this.getOut(),offset=this.getOffset(),direction=this.getDirection(),useCssTransform=this.getUseCssTransform(),reverse=this.getReverse(),translateX=0,translateY=0,fromX,fromY,toX,toY;if(reverse){direction=this.reverseDirectionMap[direction];}
switch(direction){case this.DIRECTION_UP:if(out){translateY=containerBox.top-box.top-box.height-offset;}
else{translateY=containerBox.bottom-box.bottom+box.height+offset;}
break;case this.DIRECTION_DOWN:if(out){translateY=containerBox.bottom-box.bottom+box.height+offset;}
else{translateY=containerBox.top-box.height-box.top-offset;}
break;case this.DIRECTION_RIGHT:if(out){translateX=containerBox.right-box.right+box.width+offset;}
else{translateX=containerBox.left-box.left-box.width-offset;}
break;case this.DIRECTION_LEFT:if(out){translateX=containerBox.left-box.left-box.width-offset;}
else{translateX=containerBox.right-box.right+box.width+offset;}
break;}
fromX=(out)?0:translateX;fromY=(out)?0:translateY;if(useCssTransform){from.setTransform({translateX:fromX,translateY:fromY});}
else{from.set('left',fromX);from.set('top',fromY);}
toX=(out)?translateX:0;toY=(out)?translateY:0;if(useCssTransform){to.setTransform({translateX:toX,translateY:toY});}
else{to.set('left',toX);to.set('top',toY);}
return this.callParent(arguments);}});Ext.define('Ext.fx.animation.SlideOut',{extend:'Ext.fx.animation.Slide',alias:['animation.slideOut'],config:{out:true}});Ext.define('Ext.fx.animation.Fade',{extend:'Ext.fx.animation.Abstract',alternateClassName:'Ext.fx.animation.FadeIn',alias:['animation.fade','animation.fadeIn'],config:{out:false,before:{display:null,opacity:0},after:{opacity:null},reverse:null},updateOut:function(newOut){var to=this.getTo(),from=this.getFrom();if(newOut){from.set('opacity',1);to.set('opacity',0);}else{from.set('opacity',0);to.set('opacity',1);}}});Ext.define('Ext.fx.animation.FadeOut',{extend:'Ext.fx.animation.Fade',alias:'animation.fadeOut',config:{out:true,before:{}}});Ext.define('Ext.fx.animation.Flip',{extend:'Ext.fx.animation.Abstract',alias:'animation.flip',config:{easing:'ease-in',direction:'right',half:false,out:null},getData:function(){var from=this.getFrom(),to=this.getTo(),direction=this.getDirection(),out=this.getOut(),half=this.getHalf(),rotate=(half)?90:180,fromScale=1,toScale=1,fromRotateX=0,fromRotateY=0,toRotateX=0,toRotateY=0;if(out){toScale=0.8;}
else{fromScale=0.8;}
switch(direction){case this.DIRECTION_UP:if(out){toRotateX=rotate;}
else{fromRotateX=-rotate;}
break;case this.DIRECTION_DOWN:if(out){toRotateX=-rotate;}
else{fromRotateX=rotate;}
break;case this.DIRECTION_RIGHT:if(out){toRotateY=-rotate;}
else{fromRotateY=rotate;}
break;case this.DIRECTION_LEFT:if(out){toRotateY=-rotate;}
else{fromRotateY=rotate;}
break;}
from.setTransform({rotateX:fromRotateX,rotateY:fromRotateY,scale:fromScale});to.setTransform({rotateX:toRotateX,rotateY:toRotateY,scale:toScale});return this.callParent(arguments);}});Ext.define('Ext.fx.animation.Pop',{extend:'Ext.fx.animation.Abstract',alias:['animation.pop','animation.popIn'],alternateClassName:'Ext.fx.animation.PopIn',config:{out:false,before:{display:null,opacity:0},after:{opacity:null}},getData:function(){var to=this.getTo(),from=this.getFrom(),out=this.getOut();if(out){from.set('opacity',1);from.setTransform({scale:1});to.set('opacity',0);to.setTransform({scale:0});}
else{from.set('opacity',0);from.setTransform({scale:0});to.set('opacity',1);to.setTransform({scale:1});}
return this.callParent(arguments);}});Ext.define('Ext.fx.animation.PopOut',{extend:'Ext.fx.animation.Pop',alias:'animation.popOut',config:{out:true,before:{}}});Ext.define('Ext.fx.Animation',{requires:['Ext.fx.animation.Slide','Ext.fx.animation.SlideOut','Ext.fx.animation.Fade','Ext.fx.animation.FadeOut','Ext.fx.animation.Flip','Ext.fx.animation.Pop','Ext.fx.animation.PopOut'],constructor:function(config){var defaultClass=Ext.fx.animation.Abstract,type;if(typeof config=='string'){type=config;config={};}
else if(config&&config.type){type=config.type;}
if(type){if(Ext.os.is.Android2){if(type=='pop'){type='fade';}
if(type=='popIn'){type='fadeIn';}
if(type=='popOut'){type='fadeOut';}}
defaultClass=Ext.ClassManager.getByAlias('animation.'+type);if(!defaultClass){Ext.Logger.error("Invalid animation type of: '"+type+"'");}}
return Ext.factory(config,defaultClass);}});Ext.define('Ext.AbstractComponent',{extend:'Ext.Evented',onClassExtended:function(Class,members){if(!members.hasOwnProperty('cachedConfig')){return;}
var prototype=Class.prototype,config=members.config,cachedConfig=members.cachedConfig,cachedConfigList=prototype.cachedConfigList,hasCachedConfig=prototype.hasCachedConfig,name,value;delete members.cachedConfig;prototype.cachedConfigList=cachedConfigList=(cachedConfigList)?cachedConfigList.slice():[];prototype.hasCachedConfig=hasCachedConfig=(hasCachedConfig)?Ext.Object.chain(hasCachedConfig):{};if(!config){members.config=config={};}
for(name in cachedConfig){if(cachedConfig.hasOwnProperty(name)){value=cachedConfig[name];if(!hasCachedConfig[name]){hasCachedConfig[name]=true;cachedConfigList.push(name);}
config[name]=value;}}},getElementConfig:Ext.emptyFn,referenceAttributeName:'reference',referenceSelector:'[reference]',addReferenceNode:function(name,domNode){Ext.Object.defineProperty(this,name,{get:function(){var reference;delete this[name];this[name]=reference=new Ext.Element(domNode);return reference;},configurable:true});},initElement:function(){var prototype=this.self.prototype,id=this.getId(),referenceList=[],cleanAttributes=true,referenceAttributeName=this.referenceAttributeName,needsOptimization=false,renderTemplate,renderElement,element,referenceNodes,i,ln,referenceNode,reference,configNameCache,defaultConfig,cachedConfigList,initConfigList,initConfigMap,configList,elements,name,nameMap,internalName;if(prototype.hasOwnProperty('renderTemplate')){renderTemplate=this.renderTemplate.cloneNode(true);renderElement=renderTemplate.firstChild;}
else{cleanAttributes=false;needsOptimization=true;renderTemplate=document.createDocumentFragment();renderElement=Ext.Element.create(this.getElementConfig(),true);renderTemplate.appendChild(renderElement);}
referenceNodes=renderTemplate.querySelectorAll(this.referenceSelector);for(i=0,ln=referenceNodes.length;i<ln;i++){referenceNode=referenceNodes[i];reference=referenceNode.getAttribute(referenceAttributeName);if(cleanAttributes){referenceNode.removeAttribute(referenceAttributeName);}
if(reference=='element'){referenceNode.id=id;this.element=element=new Ext.Element(referenceNode);}
else{this.addReferenceNode(reference,referenceNode);}
referenceList.push(reference);}
this.referenceList=referenceList;if(!this.innerElement){this.innerElement=element;}
if(renderElement===element.dom){this.renderElement=element;}
else{this.addReferenceNode('renderElement',renderElement);}
if(needsOptimization){configNameCache=Ext.Class.configNameCache;defaultConfig=this.config;cachedConfigList=this.cachedConfigList;initConfigList=this.initConfigList;initConfigMap=this.initConfigMap;configList=[];for(i=0,ln=cachedConfigList.length;i<ln;i++){name=cachedConfigList[i];nameMap=configNameCache[name];if(initConfigMap[name]){initConfigMap[name]=false;Ext.Array.remove(initConfigList,name);}
if(defaultConfig[name]!==null){configList.push(name);this[nameMap.get]=this[nameMap.initGet];}}
for(i=0,ln=configList.length;i<ln;i++){name=configList[i];nameMap=configNameCache[name];internalName=nameMap.internal;this[internalName]=null;this[nameMap.set].call(this,defaultConfig[name]);delete this[nameMap.get];prototype[internalName]=this[internalName];}
renderElement=this.renderElement.dom;prototype.renderTemplate=renderTemplate=document.createDocumentFragment();renderTemplate.appendChild(renderElement.cloneNode(true));elements=renderTemplate.querySelectorAll('[id]');for(i=0,ln=elements.length;i<ln;i++){element=elements[i];element.removeAttribute('id');}
for(i=0,ln=referenceList.length;i<ln;i++){reference=referenceList[i];this[reference].dom.removeAttribute('reference');}}
return this;}});(function(clsPrefix){Ext.define('Ext.layout.Default',{extend:'Ext.Evented',alternateClassName:['Ext.layout.AutoContainerLayout','Ext.layout.ContainerLayout'],alias:['layout.auto','layout.default'],isLayout:true,hasDockedItemsCls:clsPrefix+'hasdocked',centeredItemCls:clsPrefix+'centered',floatingItemCls:clsPrefix+'floating',dockingWrapperCls:clsPrefix+'docking',dockingInnerCls:clsPrefix+'docking-inner',maskCls:clsPrefix+'mask',positionMap:{top:'start',left:'start',bottom:'end',right:'end'},positionDirectionMap:{top:'vertical',bottom:'vertical',left:'horizontal',right:'horizontal'},DIRECTION_VERTICAL:'vertical',DIRECTION_HORIZONTAL:'horizontal',POSITION_START:'start',POSITION_END:'end',config:{animation:null},constructor:function(container,config){this.container=container;this.innerItems=[];this.centeringWrappers={};this.initConfig(config);},reapply:Ext.emptyFn,unapply:Ext.emptyFn,onItemAdd:function(){this.doItemAdd.apply(this,arguments);},onItemRemove:function(){this.doItemRemove.apply(this,arguments);},onItemMove:function(){this.doItemMove.apply(this,arguments);},onItemCenteredChange:function(){this.doItemCenteredChange.apply(this,arguments);},onItemFloatingChange:function(){this.doItemFloatingChange.apply(this,arguments);},onItemDockedChange:function(){this.doItemDockedChange.apply(this,arguments);},doItemAdd:function(item,index){var docked=item.getDocked();if(docked!==null){this.dockItem(item,docked);}
else if(item.isCentered()){this.centerItem(item,index);}
else{this.insertItem(item,index);}
if(item.isFloating()){this.onItemFloatingChange(item,true);}},doItemRemove:function(item){if(item.isDocked()){this.undockItem(item);}
else if(item.isCentered()){this.uncenterItem(item);}
if(item.getTranslatable()){item.setTranslatable(false);}
Ext.Array.remove(this.innerItems,item);try{this.container.innerElement.dom.removeChild(item.renderElement.dom);}catch(e){}},doItemMove:function(item,toIndex,fromIndex){if(item.isCentered()){item.setZIndex((toIndex+1)*2);}
else{if(item.isFloating()){item.setZIndex((toIndex+1)*2);}
this.insertItem(item,toIndex);}},doItemCenteredChange:function(item,centered){if(centered){this.centerItem(item);}
else{this.uncenterItem(item);}},doItemFloatingChange:function(item,floating){var element=item.element,floatingItemCls=this.floatingItemCls;if(floating){if(item.getZIndex()===null){item.setZIndex((this.container.indexOf(item)+1)*2);}
element.addCls(floatingItemCls);}
else{item.setZIndex(null);element.removeCls(floatingItemCls);}},doItemDockedChange:function(item,docked,oldDocked){if(oldDocked){this.undockItem(item,oldDocked);}
if(docked){this.dockItem(item,docked);}},centerItem:function(item){this.insertItem(item,0);if(item.getZIndex()===null){item.setZIndex((this.container.indexOf(item)+1)*2);}
this.createCenteringWrapper(item);item.element.addCls(this.floatingItemCls);},uncenterItem:function(item){this.destroyCenteringWrapper(item);item.setZIndex(null);this.insertItem(item,this.container.indexOf(item));item.element.removeCls(this.floatingItemCls);},dockItem:function(item,position){var container=this.container,itemRenderElement=item.renderElement,itemElement=item.element,dockingInnerElement=this.dockingInnerElement;if(!dockingInnerElement){container.setUseBodyElement(true);this.dockingInnerElement=dockingInnerElement=container.bodyElement;}
this.getDockingWrapper(position);if(this.positionMap[position]===this.POSITION_START){itemRenderElement.insertBefore(dockingInnerElement);}
else{itemRenderElement.insertAfter(dockingInnerElement);}
itemElement.addCls(clsPrefix+'docked-'+position);},undockItem:function(item,docked){this.insertItem(item,this.container.indexOf(item));item.element.removeCls(clsPrefix+'docked-'+docked);},getDockingWrapper:function(position){var currentDockingDirection=this.currentDockingDirection,direction=this.positionDirectionMap[position],dockingWrapper=this.dockingWrapper;if(currentDockingDirection!==direction){this.currentDockingDirection=direction;this.dockingWrapper=dockingWrapper=this.createDockingWrapper(direction);}
return dockingWrapper;},createDockingWrapper:function(direction){return this.dockingInnerElement.wrap({classList:[this.dockingWrapperCls+'-'+direction]},true);},createCenteringWrapper:function(item){var id=item.getId(),wrappers=this.centeringWrappers,renderElement=item.renderElement,wrapper;wrappers[id]=wrapper=renderElement.wrap({className:this.centeredItemCls});return wrapper;},destroyCenteringWrapper:function(item){var id=item.getId(),wrappers=this.centeringWrappers,renderElement=item.renderElement,wrapper=wrappers[id];renderElement.unwrap();wrapper.destroy();delete wrappers[id];return this;},insertItem:function(item,index){var container=this.container,items=container.getItems().items,innerItems=this.innerItems,containerDom=container.innerElement.dom,itemDom=item.renderElement.dom,relativeItem,relativeItemDom,domIndex;if(container.has(item)){Ext.Array.remove(innerItems,item);}
if(typeof index=='number'){relativeItem=items[index];if(relativeItem===item){relativeItem=items[++index];}
while(relativeItem&&(relativeItem.isCentered()||relativeItem.isDocked())){relativeItem=items[++index];}
if(relativeItem){domIndex=innerItems.indexOf(relativeItem);if(domIndex!==-1){while(relativeItem&&(relativeItem.isCentered()||relativeItem.isDocked())){relativeItem=innerItems[++domIndex];}
if(relativeItem){innerItems.splice(domIndex,0,item);relativeItemDom=relativeItem.renderElement.dom;containerDom.insertBefore(itemDom,relativeItemDom);return this;}}}}
innerItems.push(item);containerDom.appendChild(itemDom);return this;}});})(Ext.baseCSSPrefix);Ext.define('Ext.layout.Fit',{extend:'Ext.layout.Default',alternateClassName:'Ext.layout.FitLayout',alias:'layout.fit',cls:Ext.baseCSSPrefix+'layout-fit',itemCls:Ext.baseCSSPrefix+'layout-fit-item',constructor:function(container){this.callParent(arguments);this.apply();},apply:function(){this.container.innerElement.addCls(this.cls);},reapply:function(){this.apply();},unapply:function(){this.container.innerElement.removeCls(this.cls);},doItemAdd:function(item,index){if(item.isInnerItem()){item.addCls(this.itemCls);}
this.callParent(arguments);},doItemRemove:function(item){if(item.isInnerItem()){item.removeCls(this.itemCls);}
this.callParent(arguments);}});Ext.define('Ext.layout.AbstractBox',{extend:'Ext.layout.Default',config:{align:'stretch',pack:null},flexItemCls:Ext.baseCSSPrefix+'layout-box-item',positionMap:{middle:'center',left:'start',top:'start',right:'end',bottom:'end'},constructor:function(container){this.callParent(arguments);container.innerElement.addCls(this.cls);container.on(this.sizeChangeEventName,'onItemSizeChange',this,{delegate:'> component'});},reapply:function(){this.container.innerElement.addCls(this.cls);this.updatePack(this.getPack());this.updateAlign(this.getAlign());},unapply:function(){this.container.innerElement.removeCls(this.cls);this.updatePack(null);this.updateAlign(null);},doItemAdd:function(item,index){this.callParent(arguments);if(item.isInnerItem()){var size=item.getConfig(this.sizePropertyName),config=item.config;if(!size&&('flex'in config)){this.setItemFlex(item,config.flex);}}},doItemRemove:function(item){if(item.isInnerItem()){this.setItemFlex(item,null);}
this.callParent(arguments);},onItemSizeChange:function(item){this.setItemFlex(item,null);},doItemCenteredChange:function(item,centered){if(centered){this.setItemFlex(item,null);}
this.callParent(arguments);},doItemFloatingChange:function(item,floating){if(floating){this.setItemFlex(item,null);}
this.callParent(arguments);},doItemDockedChange:function(item,docked){if(docked){this.setItemFlex(item,null);}
this.callParent(arguments);},redrawContainer:function(){var container=this.container,renderedTo=container.renderElement.dom.parentNode;if(renderedTo&&renderedTo.nodeType!==11){container.innerElement.redraw();}},setItemFlex:function(item,flex){var element=item.element,flexItemCls=this.flexItemCls;if(flex){element.addCls(flexItemCls);}
else if(element.hasCls(flexItemCls)){this.redrawContainer();element.removeCls(flexItemCls);}
element.dom.style.webkitBoxFlex=flex;},convertPosition:function(position){if(this.positionMap.hasOwnProperty(position)){return this.positionMap[position];}
return position;},applyAlign:function(align){return this.convertPosition(align);},updateAlign:function(align){this.container.innerElement.dom.style.webkitBoxAlign=align;},applyPack:function(pack){return this.convertPosition(pack);},updatePack:function(pack){this.container.innerElement.dom.style.webkitBoxPack=pack;}});Ext.define('Ext.layout.HBox',{extend:'Ext.layout.AbstractBox',alternateClassName:'Ext.layout.HBoxLayout',alias:'layout.hbox',sizePropertyName:'width',sizeChangeEventName:'widthchange',cls:Ext.baseCSSPrefix+'layout-hbox'});Ext.define('Ext.layout.VBox',{extend:'Ext.layout.AbstractBox',alternateClassName:'Ext.layout.VBoxLayout',alias:'layout.vbox',sizePropertyName:'height',sizeChangeEventName:'heightchange',cls:Ext.baseCSSPrefix+'layout-vbox'});Ext.define('Ext.util.AbstractMixedCollection',{requires:['Ext.util.Filter'],mixins:{observable:'Ext.mixin.Observable'},constructor:function(allowFunctions,keyFn){var me=this;me.items=[];me.map={};me.keys=[];me.length=0;me.allowFunctions=allowFunctions===true;if(keyFn){me.getKey=keyFn;}
me.mixins.observable.constructor.call(me);},allowFunctions:false,add:function(key,obj){var me=this,myObj=obj,myKey=key,old;if(arguments.length==1){myObj=myKey;myKey=me.getKey(myObj);}
if(typeof myKey!='undefined'&&myKey!==null){old=me.map[myKey];if(typeof old!='undefined'){return me.replace(myKey,myObj);}
me.map[myKey]=myObj;}
me.length++;me.items.push(myObj);me.keys.push(myKey);me.fireEvent('add',me.length-1,myObj,myKey);return myObj;},getKey:function(o){return o.id;},replace:function(key,o){var me=this,old,index;if(arguments.length==1){o=arguments[0];key=me.getKey(o);}
old=me.map[key];if(typeof key=='undefined'||key===null||typeof old=='undefined'){return me.add(key,o);}
index=me.indexOfKey(key);me.items[index]=o;me.map[key]=o;me.fireEvent('replace',key,old,o);return o;},addAll:function(objs){var me=this,i=0,args,len,key;if(arguments.length>1||Ext.isArray(objs)){args=arguments.length>1?arguments:objs;for(len=args.length;i<len;i++){me.add(args[i]);}}else{for(key in objs){if(objs.hasOwnProperty(key)){if(me.allowFunctions||typeof objs[key]!='function'){me.add(key,objs[key]);}}}}},each:function(fn,scope){var items=[].concat(this.items),i=0,len=items.length,item;for(;i<len;i++){item=items[i];if(fn.call(scope||item,item,i,len)===false){break;}}},eachKey:function(fn,scope){var keys=this.keys,items=this.items,i=0,len=keys.length;for(;i<len;i++){fn.call(scope||window,keys[i],items[i],i,len);}},findBy:function(fn,scope){var keys=this.keys,items=this.items,i=0,len=items.length;for(;i<len;i++){if(fn.call(scope||window,items[i],keys[i])){return items[i];}}
return null;},insert:function(index,key,obj){var me=this,myKey=key,myObj=obj;if(arguments.length==2){myObj=myKey;myKey=me.getKey(myObj);}
if(me.containsKey(myKey)){me.suspendEvents();me.removeAtKey(myKey);me.resumeEvents();}
if(index>=me.length){return me.add(myKey,myObj);}
me.length++;Ext.Array.splice(me.items,index,0,myObj);if(typeof myKey!='undefined'&&myKey!==null){me.map[myKey]=myObj;}
Ext.Array.splice(me.keys,index,0,myKey);me.fireEvent('add',index,myObj,myKey);return myObj;},remove:function(o){return this.removeAt(this.indexOf(o));},removeAll:function(items){Ext.each(items||[],function(item){this.remove(item);},this);return this;},removeAt:function(index){var me=this,o,key;if(index<me.length&&index>=0){me.length--;o=me.items[index];Ext.Array.erase(me.items,index,1);key=me.keys[index];if(typeof key!='undefined'){delete me.map[key];}
Ext.Array.erase(me.keys,index,1);me.fireEvent('remove',o,key);return o;}
return false;},removeAtKey:function(key){return this.removeAt(this.indexOfKey(key));},getCount:function(){return this.length;},indexOf:function(o){return Ext.Array.indexOf(this.items,o);},indexOfKey:function(key){return Ext.Array.indexOf(this.keys,key);},get:function(key){var me=this,mk=me.map[key],item=mk!==undefined?mk:(typeof key=='number')?me.items[key]:undefined;return typeof item!='function'||me.allowFunctions?item:null;},getAt:function(index){return this.items[index];},getByKey:function(key){return this.map[key];},contains:function(o){return Ext.Array.contains(this.items,o);},containsKey:function(key){return typeof this.map[key]!='undefined';},clear:function(){var me=this;me.length=0;me.items=[];me.keys=[];me.map={};me.fireEvent('clear');},first:function(){return this.items[0];},last:function(){return this.items[this.length-1];},sum:function(property,root,start,end){var values=this.extractValues(property,root),length=values.length,sum=0,i;start=start||0;end=(end||end===0)?end:length-1;for(i=start;i<=end;i++){sum+=values[i];}
return sum;},collect:function(property,root,allowNull){var values=this.extractValues(property,root),length=values.length,hits={},unique=[],value,strValue,i;for(i=0;i<length;i++){value=values[i];strValue=String(value);if((allowNull||!Ext.isEmpty(value))&&!hits[strValue]){hits[strValue]=true;unique.push(value);}}
return unique;},extractValues:function(property,root){var values=this.items;if(root){values=Ext.Array.pluck(values,root);}
return Ext.Array.pluck(values,property);},getRange:function(start,end){var me=this,items=me.items,range=[],i;if(items.length<1){return range;}
start=start||0;end=Math.min(typeof end=='undefined'?me.length-1:end,me.length-1);if(start<=end){for(i=start;i<=end;i++){range[range.length]=items[i];}}else{for(i=start;i>=end;i--){range[range.length]=items[i];}}
return range;},filter:function(property,value,anyMatch,caseSensitive){var filters=[],filterFn;if(Ext.isString(property)){filters.push(Ext.create('Ext.util.Filter',{property:property,value:value,anyMatch:anyMatch,caseSensitive:caseSensitive}));}else if(Ext.isArray(property)||property instanceof Ext.util.Filter){filters=filters.concat(property);}
filterFn=function(record){var isMatch=true,length=filters.length,i;for(i=0;i<length;i++){var filter=filters[i],fn=filter.getFilterFn(),scope=filter.getScope();isMatch=isMatch&&fn.call(scope,record);}
return isMatch;};return this.filterBy(filterFn);},filterBy:function(fn,scope){var me=this,newMC=new this.self(),keys=me.keys,items=me.items,length=items.length,i;newMC.getKey=me.getKey;for(i=0;i<length;i++){if(fn.call(scope||me,items[i],keys[i])){newMC.add(keys[i],items[i]);}}
return newMC;},findIndex:function(property,value,start,anyMatch,caseSensitive){if(Ext.isEmpty(value,false)){return-1;}
value=this.createValueMatcher(value,anyMatch,caseSensitive);return this.findIndexBy(function(o){return o&&value.test(o[property]);},null,start);},findIndexBy:function(fn,scope,start){var me=this,keys=me.keys,items=me.items,i=start||0,len=items.length;for(;i<len;i++){if(fn.call(scope||me,items[i],keys[i])){return i;}}
return-1;},createValueMatcher:function(value,anyMatch,caseSensitive,exactMatch){if(!value.exec){var er=Ext.String.escapeRegex;value=String(value);if(anyMatch===true){value=er(value);}else{value='^'+er(value);if(exactMatch===true){value+='$';}}
value=new RegExp(value,caseSensitive?'':'i');}
return value;},clone:function(){var me=this,copy=new this.self(),keys=me.keys,items=me.items,i=0,len=items.length;for(;i<len;i++){copy.add(keys[i],items[i]);}
copy.getKey=me.getKey;return copy;}});Ext.define('Ext.util.MixedCollection',{extend:'Ext.util.AbstractMixedCollection',mixins:{sortable:'Ext.util.Sortable'},constructor:function(){var me=this;me.callParent(arguments);me.mixins.sortable.initSortable.call(me);},doSort:function(sorterFn){this.sortBy(sorterFn);},_sort:function(property,dir,fn){var me=this,i,len,dsc=String(dir).toUpperCase()=='DESC'?-1:1,c=[],keys=me.keys,items=me.items;fn=fn||function(a,b){return a-b;};for(i=0,len=items.length;i<len;i++){c[c.length]={key:keys[i],value:items[i],index:i};}
Ext.Array.sort(c,function(a,b){var v=fn(a[property],b[property])*dsc;if(v===0){v=(a.index<b.index?-1:1);}
return v;});for(i=0,len=c.length;i<len;i++){items[i]=c[i].value;keys[i]=c[i].key;}
me.fireEvent('sort',me);},sortBy:function(sorterFn){var me=this,items=me.items,keys=me.keys,length=items.length,temp=[],i;for(i=0;i<length;i++){temp[i]={key:keys[i],value:items[i],index:i};}
Ext.Array.sort(temp,function(a,b){var v=sorterFn(a.value,b.value);if(v===0){v=(a.index<b.index?-1:1);}
return v;});for(i=0;i<length;i++){items[i]=temp[i].value;keys[i]=temp[i].key;}
me.fireEvent('sort',me,items,keys);},reorder:function(mapping){var me=this,items=me.items,index=0,length=items.length,order=[],remaining=[],oldIndex;me.suspendEvents();for(oldIndex in mapping){order[mapping[oldIndex]]=items[oldIndex];}
for(index=0;index<length;index++){if(mapping[index]==undefined){remaining.push(items[index]);}}
for(index=0;index<length;index++){if(order[index]==undefined){order[index]=remaining.shift();}}
me.clear();me.addAll(order);me.resumeEvents();me.fireEvent('sort',me);},sortByKey:function(dir,fn){this._sort('key',dir,fn||function(a,b){var v1=String(a).toUpperCase(),v2=String(b).toUpperCase();return v1>v2?1:(v1<v2?-1:0);});}});Ext.define('Ext.ItemCollection',{extend:'Ext.util.MixedCollection',getKey:function(item){return item.getItemId();},has:function(item){return this.map.hasOwnProperty(item.getId());}});(function(){function xf(format){var args=Array.prototype.slice.call(arguments,1);return format.replace(/\{(\d+)\}/g,function(m,i){return args[i];});}
Ext.DateExtras={now:Date.now||function(){return+new Date();},getElapsed:function(dateA,dateB){return Math.abs(dateA-(dateB||new Date()));},useStrict:false,formatCodeToRegex:function(character,currentGroup){var p=utilDate.parseCodes[character];if(p){p=typeof p=='function'?p():p;utilDate.parseCodes[character]=p;}
return p?Ext.applyIf({c:p.c?xf(p.c,currentGroup||"{0}"):p.c},p):{g:0,c:null,s:Ext.String.escapeRegex(character)};},parseFunctions:{"MS":function(input,strict){var re=new RegExp('\\/Date\\(([-+])?(\\d+)(?:[+-]\\d{4})?\\)\\/');var r=(input||'').match(re);return r?new Date(((r[1]||'')+r[2])*1):null;}},parseRegexes:[],formatFunctions:{"MS":function(){return'\\/Date('+this.getTime()+')\\/';}},y2kYear:50,MILLI:"ms",SECOND:"s",MINUTE:"mi",HOUR:"h",DAY:"d",MONTH:"mo",YEAR:"y",defaults:{},dayNames:["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"],monthNames:["January","February","March","April","May","June","July","August","September","October","November","December"],monthNumbers:{Jan:0,Feb:1,Mar:2,Apr:3,May:4,Jun:5,Jul:6,Aug:7,Sep:8,Oct:9,Nov:10,Dec:11},defaultFormat:"m/d/Y",getShortMonthName:function(month){return utilDate.monthNames[month].substring(0,3);},getShortDayName:function(day){return utilDate.dayNames[day].substring(0,3);},getMonthNumber:function(name){return utilDate.monthNumbers[name.substring(0,1).toUpperCase()+name.substring(1,3).toLowerCase()];},formatCodes:{d:"Ext.String.leftPad(this.getDate(), 2, '0')",D:"Ext.Date.getShortDayName(this.getDay())",j:"this.getDate()",l:"Ext.Date.dayNames[this.getDay()]",N:"(this.getDay() ? this.getDay() : 7)",S:"Ext.Date.getSuffix(this)",w:"this.getDay()",z:"Ext.Date.getDayOfYear(this)",W:"Ext.String.leftPad(Ext.Date.getWeekOfYear(this), 2, '0')",F:"Ext.Date.monthNames[this.getMonth()]",m:"Ext.String.leftPad(this.getMonth() + 1, 2, '0')",M:"Ext.Date.getShortMonthName(this.getMonth())",n:"(this.getMonth() + 1)",t:"Ext.Date.getDaysInMonth(this)",L:"(Ext.Date.isLeapYear(this) ? 1 : 0)",o:"(this.getFullYear() + (Ext.Date.getWeekOfYear(this) == 1 && this.getMonth() > 0 ? +1 : (Ext.Date.getWeekOfYear(this) >= 52 && this.getMonth() < 11 ? -1 : 0)))",Y:"Ext.String.leftPad(this.getFullYear(), 4, '0')",y:"('' + this.getFullYear()).substring(2, 4)",a:"(this.getHours() < 12 ? 'am' : 'pm')",A:"(this.getHours() < 12 ? 'AM' : 'PM')",g:"((this.getHours() % 12) ? this.getHours() % 12 : 12)",G:"this.getHours()",h:"Ext.String.leftPad((this.getHours() % 12) ? this.getHours() % 12 : 12, 2, '0')",H:"Ext.String.leftPad(this.getHours(), 2, '0')",i:"Ext.String.leftPad(this.getMinutes(), 2, '0')",s:"Ext.String.leftPad(this.getSeconds(), 2, '0')",u:"Ext.String.leftPad(this.getMilliseconds(), 3, '0')",O:"Ext.Date.getGMTOffset(this)",P:"Ext.Date.getGMTOffset(this, true)",T:"Ext.Date.getTimezone(this)",Z:"(this.getTimezoneOffset() * -60)",c:function(){for(var c="Y-m-dTH:i:sP",code=[],i=0,l=c.length;i<l;++i){var e=c.charAt(i);code.push(e=="T"?"'T'":utilDate.getFormatCode(e));}
return code.join(" + ");},U:"Math.round(this.getTime() / 1000)"},isValid:function(y,m,d,h,i,s,ms){h=h||0;i=i||0;s=s||0;ms=ms||0;var dt=utilDate.add(new Date(y<100?100:y,m-1,d,h,i,s,ms),utilDate.YEAR,y<100?y-100:0);return y==dt.getFullYear()&&m==dt.getMonth()+1&&d==dt.getDate()&&h==dt.getHours()&&i==dt.getMinutes()&&s==dt.getSeconds()&&ms==dt.getMilliseconds();},parse:function(input,format,strict){var p=utilDate.parseFunctions;if(p[format]==null){utilDate.createParser(format);}
return p[format](input,Ext.isDefined(strict)?strict:utilDate.useStrict);},parseDate:function(input,format,strict){return utilDate.parse(input,format,strict);},getFormatCode:function(character){var f=utilDate.formatCodes[character];if(f){f=typeof f=='function'?f():f;utilDate.formatCodes[character]=f;}
return f||("'"+Ext.String.escape(character)+"'");},createFormat:function(format){var code=[],special=false,ch='';for(var i=0;i<format.length;++i){ch=format.charAt(i);if(!special&&ch=="\\"){special=true;}else if(special){special=false;code.push("'"+Ext.String.escape(ch)+"'");}else{code.push(utilDate.getFormatCode(ch));}}
utilDate.formatFunctions[format]=Ext.functionFactory("return "+code.join('+'));},createParser:(function(){var code=["var dt, y, m, d, h, i, s, ms, o, z, zz, u, v,","def = Ext.Date.defaults,","results = String(input).match(Ext.Date.parseRegexes[{0}]);","if(results){","{1}","if(u != null){","v = new Date(u * 1000);","}else{","dt = Ext.Date.clearTime(new Date);","y = Ext.Number.from(y, Ext.Number.from(def.y, dt.getFullYear()));","m = Ext.Number.from(m, Ext.Number.from(def.m - 1, dt.getMonth()));","d = Ext.Number.from(d, Ext.Number.from(def.d, dt.getDate()));","h  = Ext.Number.from(h, Ext.Number.from(def.h, dt.getHours()));","i  = Ext.Number.from(i, Ext.Number.from(def.i, dt.getMinutes()));","s  = Ext.Number.from(s, Ext.Number.from(def.s, dt.getSeconds()));","ms = Ext.Number.from(ms, Ext.Number.from(def.ms, dt.getMilliseconds()));","if(z >= 0 && y >= 0){","v = Ext.Date.add(new Date(y < 100 ? 100 : y, 0, 1, h, i, s, ms), Ext.Date.YEAR, y < 100 ? y - 100 : 0);","v = !strict? v : (strict === true && (z <= 364 || (Ext.Date.isLeapYear(v) && z <= 365))? Ext.Date.add(v, Ext.Date.DAY, z) : null);","}else if(strict === true && !Ext.Date.isValid(y, m + 1, d, h, i, s, ms)){","v = null;","}else{","v = Ext.Date.add(new Date(y < 100 ? 100 : y, m, d, h, i, s, ms), Ext.Date.YEAR, y < 100 ? y - 100 : 0);","}","}","}","if(v){","if(zz != null){","v = Ext.Date.add(v, Ext.Date.SECOND, -v.getTimezoneOffset() * 60 - zz);","}else if(o){","v = Ext.Date.add(v, Ext.Date.MINUTE, -v.getTimezoneOffset() + (sn == '+'? -1 : 1) * (hr * 60 + mn));","}","}","return v;"].join('\n');return function(format){var regexNum=utilDate.parseRegexes.length,currentGroup=1,calc=[],regex=[],special=false,ch="";for(var i=0;i<format.length;++i){ch=format.charAt(i);if(!special&&ch=="\\"){special=true;}else if(special){special=false;regex.push(Ext.String.escape(ch));}else{var obj=utilDate.formatCodeToRegex(ch,currentGroup);currentGroup+=obj.g;regex.push(obj.s);if(obj.g&&obj.c){calc.push(obj.c);}}}
utilDate.parseRegexes[regexNum]=new RegExp("^"+regex.join('')+"$",'i');utilDate.parseFunctions[format]=Ext.functionFactory("input","strict",xf(code,regexNum,calc.join('')));};})(),parseCodes:{d:{g:1,c:"d = parseInt(results[{0}], 10);\n",s:"(\\d{2})"},j:{g:1,c:"d = parseInt(results[{0}], 10);\n",s:"(\\d{1,2})"},D:function(){for(var a=[],i=0;i<7;a.push(utilDate.getShortDayName(i)),++i);return{g:0,c:null,s:"(?:"+a.join("|")+")"};},l:function(){return{g:0,c:null,s:"(?:"+utilDate.dayNames.join("|")+")"};},N:{g:0,c:null,s:"[1-7]"},S:{g:0,c:null,s:"(?:st|nd|rd|th)"},w:{g:0,c:null,s:"[0-6]"},z:{g:1,c:"z = parseInt(results[{0}], 10);\n",s:"(\\d{1,3})"},W:{g:0,c:null,s:"(?:\\d{2})"},F:function(){return{g:1,c:"m = parseInt(Ext.Date.getMonthNumber(results[{0}]), 10);\n",s:"("+utilDate.monthNames.join("|")+")"};},M:function(){for(var a=[],i=0;i<12;a.push(utilDate.getShortMonthName(i)),++i);return Ext.applyIf({s:"("+a.join("|")+")"},utilDate.formatCodeToRegex("F"));},m:{g:1,c:"m = parseInt(results[{0}], 10) - 1;\n",s:"(\\d{2})"},n:{g:1,c:"m = parseInt(results[{0}], 10) - 1;\n",s:"(\\d{1,2})"},t:{g:0,c:null,s:"(?:\\d{2})"},L:{g:0,c:null,s:"(?:1|0)"},o:function(){return utilDate.formatCodeToRegex("Y");},Y:{g:1,c:"y = parseInt(results[{0}], 10);\n",s:"(\\d{4})"},y:{g:1,c:"var ty = parseInt(results[{0}], 10);\n"
+"y = ty > Ext.Date.y2kYear ? 1900 + ty : 2000 + ty;\n",s:"(\\d{1,2})"},a:{g:1,c:"if (/(am)/i.test(results[{0}])) {\n"
+"if (!h || h == 12) { h = 0; }\n"
+"} else { if (!h || h < 12) { h = (h || 0) + 12; }}",s:"(am|pm|AM|PM)"},A:{g:1,c:"if (/(am)/i.test(results[{0}])) {\n"
+"if (!h || h == 12) { h = 0; }\n"
+"} else { if (!h || h < 12) { h = (h || 0) + 12; }}",s:"(AM|PM|am|pm)"},g:function(){return utilDate.formatCodeToRegex("G");},G:{g:1,c:"h = parseInt(results[{0}], 10);\n",s:"(\\d{1,2})"},h:function(){return utilDate.formatCodeToRegex("H");},H:{g:1,c:"h = parseInt(results[{0}], 10);\n",s:"(\\d{2})"},i:{g:1,c:"i = parseInt(results[{0}], 10);\n",s:"(\\d{2})"},s:{g:1,c:"s = parseInt(results[{0}], 10);\n",s:"(\\d{2})"},u:{g:1,c:"ms = results[{0}]; ms = parseInt(ms, 10)/Math.pow(10, ms.length - 3);\n",s:"(\\d+)"},O:{g:1,c:["o = results[{0}];","var sn = o.substring(0,1),","hr = o.substring(1,3)*1 + Math.floor(o.substring(3,5) / 60),","mn = o.substring(3,5) % 60;","o = ((-12 <= (hr*60 + mn)/60) && ((hr*60 + mn)/60 <= 14))? (sn + Ext.String.leftPad(hr, 2, '0') + Ext.String.leftPad(mn, 2, '0')) : null;\n"].join("\n"),s:"([+\-]\\d{4})"},P:{g:1,c:["o = results[{0}];","var sn = o.substring(0,1),","hr = o.substring(1,3)*1 + Math.floor(o.substring(4,6) / 60),","mn = o.substring(4,6) % 60;","o = ((-12 <= (hr*60 + mn)/60) && ((hr*60 + mn)/60 <= 14))? (sn + Ext.String.leftPad(hr, 2, '0') + Ext.String.leftPad(mn, 2, '0')) : null;\n"].join("\n"),s:"([+\-]\\d{2}:\\d{2})"},T:{g:0,c:null,s:"[A-Z]{1,4}"},Z:{g:1,c:"zz = results[{0}] * 1;\n"
+"zz = (-43200 <= zz && zz <= 50400)? zz : null;\n",s:"([+\-]?\\d{1,5})"},c:function(){var calc=[],arr=[utilDate.formatCodeToRegex("Y",1),utilDate.formatCodeToRegex("m",2),utilDate.formatCodeToRegex("d",3),utilDate.formatCodeToRegex("h",4),utilDate.formatCodeToRegex("i",5),utilDate.formatCodeToRegex("s",6),{c:"ms = results[7] || '0'; ms = parseInt(ms, 10)/Math.pow(10, ms.length - 3);\n"},{c:["if(results[8]) {","if(results[8] == 'Z'){","zz = 0;","}else if (results[8].indexOf(':') > -1){",utilDate.formatCodeToRegex("P",8).c,"}else{",utilDate.formatCodeToRegex("O",8).c,"}","}"].join('\n')}];for(var i=0,l=arr.length;i<l;++i){calc.push(arr[i].c);}
return{g:1,c:calc.join(""),s:[arr[0].s,"(?:","-",arr[1].s,"(?:","-",arr[2].s,"(?:","(?:T| )?",arr[3].s,":",arr[4].s,"(?::",arr[5].s,")?","(?:(?:\\.|,)(\\d+))?","(Z|(?:[-+]\\d{2}(?::)?\\d{2}))?",")?",")?",")?"].join("")};},U:{g:1,c:"u = parseInt(results[{0}], 10);\n",s:"(-?\\d+)"}},dateFormat:function(date,format){return utilDate.format(date,format);},format:function(date,format){if(utilDate.formatFunctions[format]==null){utilDate.createFormat(format);}
var result=utilDate.formatFunctions[format].call(date);return result+'';},getTimezone:function(date){return date.toString().replace(/^.* (?:\((.*)\)|([A-Z]{1,4})(?:[\-+][0-9]{4})?(?: -?\d+)?)$/,"$1$2").replace(/[^A-Z]/g,"");},getGMTOffset:function(date,colon){var offset=date.getTimezoneOffset();return(offset>0?"-":"+")
+Ext.String.leftPad(Math.floor(Math.abs(offset)/60),2,"0")
+(colon?":":"")
+Ext.String.leftPad(Math.abs(offset%60),2,"0");},getDayOfYear:function(date){var num=0,d=Ext.Date.clone(date),m=date.getMonth(),i;for(i=0,d.setDate(1),d.setMonth(0);i<m;d.setMonth(++i)){num+=utilDate.getDaysInMonth(d);}
return num+date.getDate()-1;},getWeekOfYear:(function(){var ms1d=864e5,ms7d=7*ms1d;return function(date){var DC3=Date.UTC(date.getFullYear(),date.getMonth(),date.getDate()+3)/ms1d,AWN=Math.floor(DC3/7),Wyr=new Date(AWN*ms7d).getUTCFullYear();return AWN-Math.floor(Date.UTC(Wyr,0,7)/ms7d)+1;};})(),isLeapYear:function(date){var year=date.getFullYear();return!!((year&3)==0&&(year%100||(year%400==0&&year)));},getFirstDayOfMonth:function(date){var day=(date.getDay()-(date.getDate()-1))%7;return(day<0)?(day+7):day;},getLastDayOfMonth:function(date){return utilDate.getLastDateOfMonth(date).getDay();},getFirstDateOfMonth:function(date){return new Date(date.getFullYear(),date.getMonth(),1);},getLastDateOfMonth:function(date){return new Date(date.getFullYear(),date.getMonth(),utilDate.getDaysInMonth(date));},getDaysInMonth:(function(){var daysInMonth=[31,28,31,30,31,30,31,31,30,31,30,31];return function(date){var m=date.getMonth();return m==1&&utilDate.isLeapYear(date)?29:daysInMonth[m];};})(),getSuffix:function(date){switch(date.getDate()){case 1:case 21:case 31:return"st";case 2:case 22:return"nd";case 3:case 23:return"rd";default:return"th";}},clone:function(date){return new Date(date.getTime());},isDST:function(date){return new Date(date.getFullYear(),0,1).getTimezoneOffset()!=date.getTimezoneOffset();},clearTime:function(date,clone){if(clone){return Ext.Date.clearTime(Ext.Date.clone(date));}
var d=date.getDate();date.setHours(0);date.setMinutes(0);date.setSeconds(0);date.setMilliseconds(0);if(date.getDate()!=d){for(var hr=1,c=utilDate.add(date,Ext.Date.HOUR,hr);c.getDate()!=d;hr++,c=utilDate.add(date,Ext.Date.HOUR,hr));date.setDate(d);date.setHours(c.getHours());}
return date;},add:function(date,interval,value){var d=Ext.Date.clone(date),Date=Ext.Date;if(!interval||value===0)return d;switch(interval.toLowerCase()){case Ext.Date.MILLI:d.setMilliseconds(d.getMilliseconds()+value);break;case Ext.Date.SECOND:d.setSeconds(d.getSeconds()+value);break;case Ext.Date.MINUTE:d.setMinutes(d.getMinutes()+value);break;case Ext.Date.HOUR:d.setHours(d.getHours()+value);break;case Ext.Date.DAY:d.setDate(d.getDate()+value);break;case Ext.Date.MONTH:var day=date.getDate();if(day>28){day=Math.min(day,Ext.Date.getLastDateOfMonth(Ext.Date.add(Ext.Date.getFirstDateOfMonth(date),'mo',value)).getDate());}
d.setDate(day);d.setMonth(date.getMonth()+value);break;case Ext.Date.YEAR:d.setFullYear(date.getFullYear()+value);break;}
return d;},between:function(date,start,end){var t=date.getTime();return start.getTime()<=t&&t<=end.getTime();}};var utilDate=Ext.DateExtras;Ext.apply(Ext.Date,utilDate);Ext.apply(Ext.util.Date,utilDate);})();Ext.define('Ext.util.Format',{requires:['Ext.DateExtras'],singleton:true,defaultDateFormat:'m/d/Y',escapeRe:/('|\\)/g,trimRe:/^[\x09\x0a\x0b\x0c\x0d\x20\xa0\u1680\u180e\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u2028\u2029\u202f\u205f\u3000]+|[\x09\x0a\x0b\x0c\x0d\x20\xa0\u1680\u180e\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u2028\u2029\u202f\u205f\u3000]+$/g,formatRe:/\{(\d+)\}/g,escapeRegexRe:/([-.*+?^${}()|[\]\/\\])/g,dashesRe:/-/g,iso8601TestRe:/\d\dT\d\d/,iso8601SplitRe:/[- :T\.Z\+]/,ellipsis:function(value,len,word){if(value&&value.length>len){if(word){var vs=value.substr(0,len-2),index=Math.max(vs.lastIndexOf(' '),vs.lastIndexOf('.'),vs.lastIndexOf('!'),vs.lastIndexOf('?'));if(index!=-1&&index>=(len-15)){return vs.substr(0,index)+"...";}}
return value.substr(0,len-3)+"...";}
return value;},escapeRegex:function(s){return s.replace(Ext.util.Format.escapeRegexRe,"\\$1");},escape:function(string){return string.replace(Ext.util.Format.escapeRe,"\\$1");},toggle:function(string,value,other){return string==value?other:value;},trim:function(string){return string.replace(Ext.util.Format.trimRe,"");},leftPad:function(val,size,ch){var result=String(val);ch=ch||" ";while(result.length<size){result=ch+result;}
return result;},format:function(format){var args=Ext.toArray(arguments,1);return format.replace(Ext.util.Format.formatRe,function(m,i){return args[i];});},htmlEncode:function(value){return!value?value:String(value).replace(/&/g,"&amp;").replace(/>/g,"&gt;").replace(/</g,"&lt;").replace(/"/g,"&quot;");},htmlDecode:function(value){return!value?value:String(value).replace(/&gt;/g,">").replace(/&lt;/g,"<").replace(/&quot;/g,'"').replace(/&amp;/g,"&");},date:function(value,format){var date=value;if(!value){return"";}
if(!Ext.isDate(value)){date=new Date(Date.parse(value));if(isNaN(date)){if(this.iso8601TestRe.test(value)){date=value.split(this.iso8601SplitRe);date=new Date(date[0],date[1]-1,date[2],date[3],date[4],date[5]);}
if(isNaN(date)){date=new Date(Date.parse(value.replace(this.dashesRe,"/")));if(isNaN(date)){Ext.Logger.error("Cannot parse the passed value "+value+" into a valid date");}}}
value=date;}
return Ext.Date.format(value,format||Ext.util.Format.defaultDateFormat);}});Ext.define('Ext.Template',{requires:['Ext.dom.Helper','Ext.util.Format'],inheritableStatics:{from:function(el,config){el=Ext.getDom(el);return new this(el.value||el.innerHTML,config||'');}},constructor:function(html){var me=this,args=arguments,buffer=[],i=0,length=args.length,value;me.initialConfig={};if(length>1){for(;i<length;i++){value=args[i];if(typeof value=='object'){Ext.apply(me.initialConfig,value);Ext.apply(me,value);}else{buffer.push(value);}}
html=buffer.join('');}else{if(Ext.isArray(html)){buffer.push(html.join(''));}else{buffer.push(html);}}
me.html=buffer.join('');if(me.compiled){me.compile();}},isTemplate:true,disableFormats:false,re:/\{([\w\-]+)(?:\:([\w\.]*)(?:\((.*?)?\))?)?\}/g,apply:function(values){var me=this,useFormat=me.disableFormats!==true,fm=Ext.util.Format,tpl=me,ret;if(me.compiled){return me.compiled(values).join('');}
function fn(m,name,format,args){if(format&&useFormat){if(args){args=[values[name]].concat(Ext.functionFactory('return ['+args+'];')());}else{args=[values[name]];}
if(format.substr(0,5)=="this."){return tpl[format.substr(5)].apply(tpl,args);}
else{return fm[format].apply(fm,args);}}
else{return values[name]!==undefined?values[name]:"";}}
ret=me.html.replace(me.re,fn);return ret;},applyOut:function(values,out){var me=this;if(me.compiled){out.push.apply(out,me.compiled(values));}else{out.push(me.apply(values));}
return out;},applyTemplate:function(){return this.apply.apply(this,arguments);},set:function(html,compile){var me=this;me.html=html;me.compiled=null;return compile?me.compile():me;},compileARe:/\\/g,compileBRe:/(\r\n|\n)/g,compileCRe:/'/g,compile:function(){var me=this,fm=Ext.util.Format,useFormat=me.disableFormats!==true,body,bodyReturn;function fn(m,name,format,args){if(format&&useFormat){args=args?','+args:"";if(format.substr(0,5)!="this."){format="fm."+format+'(';}
else{format='this.'+format.substr(5)+'(';}}
else{args='';format="(values['"+name+"'] == undefined ? '' : ";}
return"',"+format+"values['"+name+"']"+args+") ,'";}
bodyReturn=me.html.replace(me.compileARe,'\\\\').replace(me.compileBRe,'\\n').replace(me.compileCRe,"\\'").replace(me.re,fn);body="this.compiled = function(values){ return ['"+bodyReturn+"'];};";eval(body);return me;},insertFirst:function(el,values,returnElement){return this.doInsert('afterBegin',el,values,returnElement);},insertBefore:function(el,values,returnElement){return this.doInsert('beforeBegin',el,values,returnElement);},insertAfter:function(el,values,returnElement){return this.doInsert('afterEnd',el,values,returnElement);},append:function(el,values,returnElement){return this.doInsert('beforeEnd',el,values,returnElement);},doInsert:function(where,el,values,returnEl){el=Ext.getDom(el);var newNode=Ext.DomHelper.insertHtml(where,el,this.apply(values));return returnEl?Ext.get(newNode,true):newNode;},overwrite:function(el,values,returnElement){el=Ext.getDom(el);el.innerHTML=this.apply(values);return returnElement?Ext.get(el.firstChild,true):el.firstChild;}});Ext.define('Ext.XTemplate',{extend:'Ext.Template',requires:'Ext.XTemplateCompiler',apply:function(values){return this.applyOut(values,[]).join('');},applyOut:function(values,out){var me=this,compiler;if(!me.fn){compiler=new Ext.XTemplateCompiler({useFormat:me.disableFormats!==true});me.fn=compiler.compile(me.html);}
try{me.fn.call(me,out,values,{},1,1);}catch(e){Ext.Logger.error(e.message);}
return out;},compile:function(){return this;},statics:{getTpl:function(instance,name){var tpl=instance[name],proto;if(tpl&&!tpl.isTemplate){tpl=Ext.ClassManager.dynInstantiate('Ext.XTemplate',tpl);if(instance.hasOwnProperty(name)){instance[name]=tpl;}else{for(proto=instance.self.prototype;proto;proto=proto.superclass){if(proto.hasOwnProperty(name)){proto[name]=tpl;break;}}}}
return tpl||null;}}});Ext.define('Ext.util.SizeMonitor',{extend:'Ext.Evented',config:{element:null,detectorCls:Ext.baseCSSPrefix+'size-change-detector',callback:Ext.emptyFn,scope:null,args:[]},constructor:function(config){this.initConfig(config);this.doFireSizeChangeEvent=Ext.Function.bind(this.doFireSizeChangeEvent,this);var me=this,element=this.getElement().dom,cls=this.getDetectorCls(),expandDetector=Ext.Element.create({classList:[cls,cls+'-expand'],children:[{}]},true),shrinkDetector=Ext.Element.create({classList:[cls,cls+'-shrink'],children:[{}]},true),expandListener=function(e){me.onDetectorScroll('expand',e);},shrinkListener=function(e){me.onDetectorScroll('shrink',e);};element.appendChild(expandDetector);element.appendChild(shrinkDetector);this.detectors={expand:expandDetector,shrink:shrinkDetector};this.position={expand:{left:0,top:0},shrink:{left:0,top:0}};this.listeners={expand:expandListener,shrink:shrinkListener};this.refresh();expandDetector.addEventListener('scroll',expandListener,true);shrinkDetector.addEventListener('scroll',shrinkListener,true);},applyElement:function(element){if(element){return Ext.get(element);}},refreshPosition:function(name){var detector=this.detectors[name],position=this.position[name],left,top;position.left=left=detector.scrollWidth-detector.offsetWidth;position.top=top=detector.scrollHeight-detector.offsetHeight;detector.scrollLeft=left;detector.scrollTop=top;},refresh:function(){this.refreshPosition('expand');this.refreshPosition('shrink');},onDetectorScroll:function(name){var detector=this.detectors[name],position=this.position[name];if(detector.scrollLeft!==position.left||detector.scrollTop!==position.top){this.refresh();this.fireSizeChangeEvent();}},fireSizeChangeEvent:function(){clearTimeout(this.sizeChangeThrottleTimer);this.sizeChangeThrottleTimer=setTimeout(this.doFireSizeChangeEvent,1);},doFireSizeChangeEvent:function(){this.getCallback().apply(this.getScope(),this.getArgs());},destroyDetector:function(name){var detector=this.detectors[name],listener=this.listeners[name];detector.removeEventListener('scroll',listener,true);Ext.removeNode(detector);},destroy:function(){this.callParent(arguments);this.destroyDetector('expand');this.destroyDetector('shrink');delete this.listeners;delete this.detectors;}});Ext.define('Ext.fx.easing.BoundMomentum',{extend:'Ext.fx.easing.Abstract',requires:['Ext.fx.easing.Momentum','Ext.fx.easing.Bounce'],config:{momentum:null,bounce:null,minMomentumValue:0,maxMomentumValue:0,minVelocity:0.01,startVelocity:0},applyMomentum:function(config,currentEasing){return Ext.factory(config,Ext.fx.easing.Momentum,currentEasing);},applyBounce:function(config,currentEasing){return Ext.factory(config,Ext.fx.easing.Bounce,currentEasing);},updateStartTime:function(startTime){this.getMomentum().setStartTime(startTime);this.callParent(arguments);},updateStartVelocity:function(startVelocity){this.getMomentum().setStartVelocity(startVelocity);},updateStartValue:function(startValue){this.getMomentum().setStartValue(startValue);},reset:function(){this.lastValue=null;this.isBouncingBack=false;this.isOutOfBound=false;return this.callParent(arguments);},getValue:function(){var momentum=this.getMomentum(),bounce=this.getBounce(),startVelocity=momentum.getStartVelocity(),direction=startVelocity>0?1:-1,minValue=this.getMinMomentumValue(),maxValue=this.getMaxMomentumValue(),boundedValue=(direction==1)?maxValue:minValue,lastValue=this.lastValue,value,velocity;if(startVelocity===0){return this.getStartValue();}
if(!this.isOutOfBound){value=momentum.getValue();velocity=momentum.getVelocity();if(Math.abs(velocity)<this.getMinVelocity()){this.isEnded=true;}
if(value>=minValue&&value<=maxValue){return value;}
this.isOutOfBound=true;bounce.setStartTime(Ext.Date.now()).setStartVelocity(velocity).setStartValue(boundedValue);}
value=bounce.getValue();if(!this.isEnded){if(!this.isBouncingBack){if(lastValue!==null){if((direction==1&&value<lastValue)||(direction==-1&&value>lastValue)){this.isBouncingBack=true;}}}
else{if(Math.round(value)==boundedValue){this.isEnded=true;}}}
this.lastValue=value;return value;}});Ext.define('Ext.fx.layout.card.Abstract',{extend:'Ext.Evented',isAnimation:true,config:{direction:'left',duration:null,reverse:null,layout:null},updateLayout:function(){this.enable();},enable:function(){var layout=this.getLayout();if(layout){layout.onBefore('activeitemchange','onActiveItemChange',this);}},disable:function(){var layout=this.getLayout();if(this.isAnimating){this.stopAnimation();}
if(layout){layout.unBefore('activeitemchange','onActiveItemChange',this);}},onActiveItemChange:Ext.emptyFn,destroy:function(){var layout=this.getLayout();if(this.isAnimating){this.stopAnimation();}
if(layout){layout.unBefore('activeitemchange','onActiveItemChange',this);}
this.setLayout(null);}});Ext.define('Ext.fx.layout.card.Style',{extend:'Ext.fx.layout.card.Abstract',requires:['Ext.fx.Animation'],config:{inAnimation:{before:{visibility:null},preserveEndState:false,replacePrevious:true},outAnimation:{preserveEndState:false,replacePrevious:true}},constructor:function(config){var inAnimation,outAnimation;this.initConfig(config);this.endAnimationCounter=0;inAnimation=this.getInAnimation();outAnimation=this.getOutAnimation();inAnimation.on('animationend','incrementEnd',this);outAnimation.on('animationend','incrementEnd',this);inAnimation.setConfig(config);outAnimation.setConfig(config);},incrementEnd:function(){this.endAnimationCounter++;if(this.endAnimationCounter>1){this.endAnimationCounter=0;this.fireEvent('animationend',this);}},applyInAnimation:function(animation,inAnimation){return Ext.factory(animation,Ext.fx.Animation,inAnimation);},applyOutAnimation:function(animation,outAnimation){return Ext.factory(animation,Ext.fx.Animation,outAnimation);},updateInAnimation:function(animation){animation.setScope(this);},updateOutAnimation:function(animation){animation.setScope(this);},onActiveItemChange:function(cardLayout,newItem,oldItem,options,controller){var inAnimation=this.getInAnimation(),outAnimation=this.getOutAnimation(),inElement,outElement;if(newItem&&oldItem&&oldItem.isPainted()){inElement=newItem.renderElement;outElement=oldItem.renderElement;inAnimation.setElement(inElement);outAnimation.setElement(outElement);outAnimation.setOnBeforeEnd(function(element,interrupted){if(interrupted||Ext.Animator.hasRunningAnimations(element)){controller.firingArguments[1]=null;controller.firingArguments[2]=null;}});outAnimation.setOnEnd(function(){controller.resume();});inElement.dom.style.setProperty('visibility','hidden','!important');newItem.show();Ext.Animator.run([outAnimation,inAnimation]);controller.pause();}}});Ext.define('Ext.fx.layout.card.Slide',{extend:'Ext.fx.layout.card.Style',alias:'fx.layout.card.slide',config:{inAnimation:{type:'slide',easing:'ease-out'},outAnimation:{type:'slide',easing:'ease-out',out:true}},updateReverse:function(reverse){this.getInAnimation().setReverse(reverse);this.getOutAnimation().setReverse(reverse);}});Ext.define('Ext.fx.layout.card.Cover',{extend:'Ext.fx.layout.card.Style',alias:'fx.layout.card.cover',config:{reverse:null,inAnimation:{before:{'z-index':100},after:{'z-index':0},type:'slide',easing:'ease-out'},outAnimation:{easing:'ease-out',from:{opacity:0.99},to:{opacity:1},out:true}},updateReverse:function(reverse){this.getInAnimation().setReverse(reverse);this.getOutAnimation().setReverse(reverse);}});Ext.define('Ext.fx.layout.card.Reveal',{extend:'Ext.fx.layout.card.Style',alias:'fx.layout.card.reveal',config:{inAnimation:{easing:'ease-out',from:{opacity:0.99},to:{opacity:1}},outAnimation:{before:{'z-index':100},after:{'z-index':0},type:'slide',easing:'ease-out',out:true}},updateReverse:function(reverse){this.getInAnimation().setReverse(reverse);this.getOutAnimation().setReverse(reverse);}});Ext.define('Ext.fx.layout.card.Fade',{extend:'Ext.fx.layout.card.Style',alias:'fx.layout.card.fade',config:{reverse:null,inAnimation:{type:'fade',easing:'ease-out'},outAnimation:{type:'fade',easing:'ease-out',out:true}}});Ext.define('Ext.fx.layout.card.Flip',{extend:'Ext.fx.layout.card.Style',alias:'fx.layout.card.flip',config:{duration:500,inAnimation:{type:'flip',half:true,easing:'ease-out',before:{'backface-visibility':'hidden'},after:{'backface-visibility':null}},outAnimation:{type:'flip',half:true,easing:'ease-in',before:{'backface-visibility':'hidden'},after:{'backface-visibility':null},out:true}},updateDuration:function(duration){var halfDuration=duration/2,inAnimation=this.getInAnimation(),outAnimation=this.getOutAnimation();inAnimation.setDelay(halfDuration);inAnimation.setDuration(halfDuration);outAnimation.setDuration(halfDuration);}});Ext.define('Ext.fx.layout.card.Pop',{extend:'Ext.fx.layout.card.Style',alias:'fx.layout.card.pop',config:{duration:500,inAnimation:{type:'pop',easing:'ease-out'},outAnimation:{type:'pop',easing:'ease-in',out:true}},updateDuration:function(duration){var halfDuration=duration/2,inAnimation=this.getInAnimation(),outAnimation=this.getOutAnimation();inAnimation.setDelay(halfDuration);inAnimation.setDuration(halfDuration);outAnimation.setDuration(halfDuration);}});Ext.define('Ext.fx.easing.Linear',{extend:'Ext.fx.easing.Abstract',alias:'easing.linear',config:{duration:0,endValue:0},updateStartValue:function(startValue){this.distance=this.getEndValue()-startValue;},updateEndValue:function(endValue){this.distance=endValue-this.getStartValue();},getValue:function(){var deltaTime=Ext.Date.now()-this.getStartTime(),duration=this.getDuration();if(deltaTime>duration){this.isEnded=true;return this.getEndValue();}
else{return this.getStartValue()+((deltaTime/duration)*this.distance);}}});Ext.define('Ext.fx.layout.card.Scroll',{extend:'Ext.fx.layout.card.Abstract',requires:['Ext.fx.easing.Linear'],alias:'fx.layout.card.scroll',config:{duration:150},constructor:function(config){this.initConfig(config);this.doAnimationFrame=Ext.Function.bind(this.doAnimationFrame,this);},getEasing:function(){var easing=this.easing;if(!easing){this.easing=easing=new Ext.fx.easing.Linear();}
return easing;},updateDuration:function(duration){this.getEasing().setDuration(duration);},onActiveItemChange:function(cardLayout,newItem,oldItem,options,controller){var direction=this.getDirection(),easing=this.getEasing(),containerElement,inElement,outElement,containerWidth,containerHeight,reverse;if(newItem&&oldItem){if(this.isAnimating){this.stopAnimation();}
containerElement=this.getLayout().container.innerElement;containerWidth=containerElement.getWidth();containerHeight=containerElement.getHeight();inElement=newItem.renderElement;outElement=oldItem.renderElement;this.oldItem=oldItem;this.newItem=newItem;this.currentEventController=controller;this.containerElement=containerElement;this.isReverse=reverse=this.getReverse();newItem.show();if(direction=='right'){direction='left';this.isReverse=reverse=!reverse;}
else if(direction=='down'){direction='up';this.isReverse=reverse=!reverse;}
if(direction=='left'){if(reverse){easing.setConfig({startValue:containerWidth,endValue:0});containerElement.dom.scrollLeft=containerWidth;outElement.setLeft(containerWidth);}
else{easing.setConfig({startValue:0,endValue:containerWidth});inElement.setLeft(containerWidth);}}
else{if(reverse){easing.setConfig({startValue:containerHeight,endValue:0});containerElement.dom.scrollTop=containerHeight;outElement.setTop(containerHeight);}
else{easing.setConfig({startValue:0,endValue:containerHeight});inElement.setTop(containerHeight);}}
this.startAnimation();controller.pause();}},startAnimation:function(){this.isAnimating=true;this.getEasing().setStartTime(Date.now());this.timer=setInterval(this.doAnimationFrame,20);this.doAnimationFrame();},doAnimationFrame:function(){var easing=this.getEasing(),direction=this.getDirection(),scroll='scrollTop',value;if(direction=='left'||direction=='right'){scroll='scrollLeft';}
if(easing.isEnded){this.stopAnimation();}
else{value=easing.getValue();this.containerElement.dom[scroll]=value;}},stopAnimation:function(){var direction=this.getDirection(),scroll='setTop';if(direction=='left'||direction=='right'){scroll='setLeft';}
this.currentEventController.resume();if(this.isReverse){this.oldItem.renderElement[scroll](null);}
else{this.newItem.renderElement[scroll](null);}
clearInterval(this.timer);this.isAnimating=false;this.fireEvent('animationend',this);}});Ext.define('Ext.fx.layout.Card',{requires:['Ext.fx.layout.card.Slide','Ext.fx.layout.card.Cover','Ext.fx.layout.card.Reveal','Ext.fx.layout.card.Fade','Ext.fx.layout.card.Flip','Ext.fx.layout.card.Pop','Ext.fx.layout.card.Scroll'],constructor:function(config){var defaultClass=Ext.fx.layout.card.Abstract,type;if(!config){return null;}
if(typeof config=='string'){type=config;config={};}
else if(config.type){type=config.type;}
config.elementBox=false;if(type){if(Ext.os.is.Android2){if(type!='fade'){type='scroll';}}
else if(type==='slide'&&Ext.browser.is.ChromeMobile){type='scroll';}
defaultClass=Ext.ClassManager.getByAlias('fx.layout.card.'+type);if(!defaultClass){Ext.Logger.error("Unknown card animation type: '"+type+"'");}}
return Ext.factory(config,defaultClass);}});Ext.define('Ext.layout.Card',{extend:'Ext.layout.Fit',alternateClassName:'Ext.layout.CardLayout',isCard:true,requires:['Ext.fx.layout.Card'],alias:'layout.card',cls:Ext.baseCSSPrefix+'layout-card',itemCls:Ext.baseCSSPrefix+'layout-card-item',constructor:function(){this.callParent(arguments);this.container.onInitialized(this.onContainerInitialized,this);},applyAnimation:function(animation){return new Ext.fx.layout.Card(animation);},updateAnimation:function(animation,oldAnimation){if(animation&&animation.isAnimation){animation.setLayout(this);}
if(oldAnimation){oldAnimation.destroy();}},doItemAdd:function(item,index){if(item.isInnerItem()){item.hide();}
this.callParent(arguments);},doItemRemove:function(item,index,destroy){this.callParent(arguments);if(!destroy&&item.isInnerItem()){item.show();}},onContainerInitialized:function(container){var activeItem=container.getActiveItem();if(activeItem){activeItem.show();}
container.on('activeitemchange','onContainerActiveItemChange',this);},onContainerActiveItemChange:function(container){this.relayEvent(arguments,'doActiveItemChange');},doActiveItemChange:function(me,newActiveItem,oldActiveItem){if(oldActiveItem){oldActiveItem.hide();}
if(newActiveItem){newActiveItem.show();}},doItemDockedChange:function(item,docked){var element=item.element;if(docked){element.removeCls(this.itemCls);}
else{element.addCls(this.itemCls);}
this.callParent(arguments);}});Ext.define('Ext.layout.Layout',{requires:['Ext.layout.Fit','Ext.layout.Card','Ext.layout.HBox','Ext.layout.VBox'],constructor:function(container,config){var layoutClass=Ext.layout.Default,type,layout;if(typeof config=='string'){type=config;config={};}
else if('type'in config){type=config.type;}
if(type){layoutClass=Ext.ClassManager.getByAlias('layout.'+type);if(!layoutClass){Ext.Logger.error("Unknown layout type of: '"+type+"'");}}
return new layoutClass(container,config);}});Ext.define('Ext.fx.easing.EaseOut',{extend:'Ext.fx.easing.Linear',alias:'easing.ease-out',config:{exponent:4,duration:1500},getValue:function(){var deltaTime=Ext.Date.now()-this.getStartTime(),duration=this.getDuration(),startValue=this.getStartValue(),endValue=this.getEndValue(),distance=this.distance,theta=deltaTime/duration,thetaC=1-theta,thetaEnd=1-Math.pow(thetaC,this.getExponent()),currentValue=startValue+(thetaEnd*distance);if(deltaTime>=duration){this.isEnded=true;return endValue;}
return currentValue;}});Ext.define('Ext.util.translatable.Abstract',{extend:'Ext.Evented',requires:['Ext.fx.easing.Linear'],config:{element:null,easing:null,easingX:null,easingY:null,fps:60},constructor:function(config){var element;this.doAnimationFrame=Ext.Function.bind(this.doAnimationFrame,this);this.x=0;this.y=0;this.activeEasingX=null;this.activeEasingY=null;this.initialConfig=config;if(config&&config.element){element=config.element;this.setElement(element);}},applyElement:function(element){if(!element){return;}
return Ext.get(element);},updateElement:function(element){this.initConfig(this.initialConfig);this.refresh();},factoryEasing:function(easing){return Ext.factory(easing,Ext.fx.easing.Linear,null,'easing');},applyEasing:function(easing){if(!this.getEasingX()){this.setEasingX(this.factoryEasing(easing));}
if(!this.getEasingY()){this.setEasingY(this.factoryEasing(easing));}},applyEasingX:function(easing){return this.factoryEasing(easing);},applyEasingY:function(easing){return this.factoryEasing(easing);},updateFps:function(fps){this.animationInterval=1000/fps;},doTranslate:function(x,y){if(typeof x=='number'){this.x=x;}
if(typeof y=='number'){this.y=y;}
return this;},translate:function(x,y,animation){if(!this.getElement().dom){return;}
if(Ext.isObject(x)){throw new Error();}
this.stopAnimation();if(animation!==undefined){return this.translateAnimated(x,y,animation);}
return this.doTranslate(x,y);},animate:function(easingX,easingY){this.activeEasingX=easingX;this.activeEasingY=easingY;this.isAnimating=true;this.animationTimer=setInterval(this.doAnimationFrame,this.animationInterval);this.fireEvent('animationstart',this,this.x,this.y);return this;},translateAnimated:function(x,y,animation){if(Ext.isObject(x)){throw new Error();}
if(!Ext.isObject(animation)){animation={};}
var now=Ext.Date.now(),easing=animation.easing,easingX=(typeof x=='number')?(animation.easingX||this.getEasingX()||easing||true):null,easingY=(typeof y=='number')?(animation.easingY||this.getEasingY()||easing||true):null;if(easingX){easingX=this.factoryEasing(easingX);easingX.setStartTime(now);easingX.setStartValue(this.x);easingX.setEndValue(x);if('duration'in animation){easingX.setDuration(animation.duration);}}
if(easingY){easingY=this.factoryEasing(easingY);easingY.setStartTime(now);easingY.setStartValue(this.y);easingY.setEndValue(y);if('duration'in animation){easingY.setDuration(animation.duration);}}
return this.animate(easingX,easingY);},doAnimationFrame:function(){var easingX=this.activeEasingX,easingY=this.activeEasingY,element=this.getElement(),x,y;if(!this.isAnimating||!element.dom){return;}
if(easingX===null&&easingY===null){this.stopAnimation();return;}
if(easingX!==null){this.x=x=Math.round(easingX.getValue());if(easingX.isEnded){this.activeEasingX=null;this.fireEvent('axisanimationend',this,'x',x);}}
else{x=this.x;}
if(easingY!==null){this.y=y=Math.round(easingY.getValue());if(easingY.isEnded){this.activeEasingY=null;this.fireEvent('axisanimationend',this,'y',y);}}
else{y=this.y;}
this.doTranslate(x,y);this.fireEvent('animationframe',this,x,y);},stopAnimation:function(){if(!this.isAnimating){return;}
this.activeEasingX=null;this.activeEasingY=null;this.isAnimating=false;clearInterval(this.animationTimer);this.fireEvent('animationend',this,this.x,this.y);},refresh:function(){this.translate(this.x,this.y);}});Ext.define('Ext.util.translatable.CssTransform',{extend:'Ext.util.translatable.Abstract',doTranslate:function(x,y){var domStyle=this.getElement().dom.style;if(typeof x!='number'){x=this.x;}
if(typeof y!='number'){y=this.y;}
domStyle.webkitTransform='translate3d('+x+'px, '+y+'px, 0px)';return this.callParent(arguments);},destroy:function(){var element=this.getElement();if(element&&!element.isDestroyed){element.dom.style.webkitTransform=null;}
this.callParent(arguments);}});Ext.define('Ext.util.translatable.ScrollPosition',{extend:'Ext.util.translatable.Abstract',wrapperWidth:0,wrapperHeight:0,baseCls:'x-translatable',config:{useWrapper:true},getWrapper:function(){var wrapper=this.wrapper,baseCls=this.baseCls,element=this.getElement(),nestedStretcher,container;if(!wrapper){container=element.getParent();if(!container){return null;}
if(this.getUseWrapper()){wrapper=element.wrap({className:baseCls+'-wrapper'},true);}
else{wrapper=container.dom;}
wrapper.appendChild(Ext.Element.create({className:baseCls+'-stretcher'},true));this.nestedStretcher=nestedStretcher=Ext.Element.create({className:baseCls+'-nested-stretcher'},true);element.appendChild(nestedStretcher);element.addCls(baseCls);container.addCls(baseCls+'-container');this.container=container;this.wrapper=wrapper;this.refresh();}
return wrapper;},doTranslate:function(x,y){var wrapper=this.getWrapper();if(wrapper){if(typeof x=='number'){wrapper.scrollLeft=this.wrapperWidth-x;}
if(typeof y=='number'){wrapper.scrollTop=this.wrapperHeight-y;}}
return this.callParent(arguments);},refresh:function(){var wrapper=this.getWrapper();if(wrapper){this.wrapperWidth=wrapper.offsetWidth;this.wrapperHeight=wrapper.offsetHeight;this.callParent(arguments);}},destroy:function(){var element=this.getElement(),baseCls=this.baseCls;if(this.wrapper){if(this.getUseWrapper()){element.unwrap();}
this.container.removeCls(baseCls+'-container');element.removeCls(baseCls);element.removeChild(this.nestedStretcher);}
this.callParent(arguments);}});Ext.define('Ext.util.Translatable',{requires:['Ext.util.translatable.CssTransform','Ext.util.translatable.ScrollPosition'],constructor:function(config){var namespace=Ext.util.translatable,CssTransform=namespace.CssTransform,ScrollPosition=namespace.ScrollPosition,classReference;if(typeof config=='object'&&'translationMethod'in config){if(config.translationMethod==='scrollposition'){classReference=ScrollPosition;}
else if(config.translationMethod==='csstransform'){classReference=CssTransform;}}
if(!classReference){if(Ext.os.is.Android2||Ext.browser.is.ChromeMobile){classReference=ScrollPosition;}
else{classReference=CssTransform;}}
return new classReference(config);}});Ext.define('Ext.behavior.Translatable',{extend:'Ext.behavior.Behavior',requires:['Ext.util.Translatable'],constructor:function(){this.listeners={painted:'onComponentPainted',scope:this};this.callParent(arguments);},onComponentPainted:function(){this.translatable.refresh();},setConfig:function(config){var translatable=this.translatable,component=this.component;if(config){if(!translatable){this.translatable=translatable=new Ext.util.Translatable(config);translatable.setElement(component.renderElement);translatable.on('destroy','onTranslatableDestroy',this);if(component.isPainted()){this.onComponentPainted(component);}
component.on(this.listeners);}
else if(Ext.isObject(config)){translatable.setConfig(config);}}
else if(translatable){translatable.destroy();}
return this;},getTranslatable:function(){return this.translatable;},onTranslatableDestroy:function(){var component=this.component;delete this.translatable;component.un(this.listeners);},onComponentDestroy:function(){var translatable=this.translatable;if(translatable){translatable.destroy();}}});Ext.define('Ext.util.Draggable',{isDraggable:true,mixins:['Ext.mixin.Observable'],requires:['Ext.util.SizeMonitor','Ext.util.Translatable'],config:{cls:Ext.baseCSSPrefix+'draggable',draggingCls:Ext.baseCSSPrefix+'dragging',element:null,constraint:'container',disabled:null,direction:'both',initialOffset:{x:0,y:0},translatable:{}},DIRECTION_BOTH:'both',DIRECTION_VERTICAL:'vertical',DIRECTION_HORIZONTAL:'horizontal',defaultConstraint:{min:{x:-Infinity,y:-Infinity},max:{x:Infinity,y:Infinity}},constructor:function(config){var element;this.sizeMonitors={};this.extraConstraint={};this.initialConfig=config;this.offset={x:0,y:0};this.listeners={dragstart:'onDragStart',drag:'onDrag',dragend:'onDragEnd',scope:this};if(config&&config.element){element=config.element;delete config.element;this.setElement(element);}
return this;},applyElement:function(element){if(!element){return;}
return Ext.get(element);},updateElement:function(element){element.on(this.listeners);this.sizeMonitors.element=new Ext.util.SizeMonitor({element:element,callback:this.doRefresh,scope:this});this.initConfig(this.initialConfig);},updateInitialOffset:function(initialOffset){if(typeof initialOffset=='number'){initialOffset={x:initialOffset,y:initialOffset};}
var offset=this.offset,x,y;offset.x=x=initialOffset.x;offset.y=y=initialOffset.y;this.getTranslatable().doTranslate(x,y);},updateCls:function(cls){this.getElement().addCls(cls);},applyTranslatable:function(translatable,currentInstance){translatable=Ext.factory(translatable,Ext.util.Translatable,currentInstance);translatable.setElement(this.getElement());return translatable;},setExtraConstraint:function(constraint){this.extraConstraint=constraint||{};this.refreshConstraint();return this;},addExtraConstraint:function(constraint){Ext.merge(this.extraConstraint,constraint);this.refreshConstraint();return this;},applyConstraint:function(newConstraint){this.currentConstraint=newConstraint;if(!newConstraint){newConstraint=this.defaultConstraint;}
if(newConstraint==='container'){return Ext.merge(this.getContainerConstraint(),this.extraConstraint);}
return Ext.merge({},this.extraConstraint,newConstraint);},updateConstraint:function(){this.refreshOffset();},getContainerConstraint:function(){var container=this.getContainer(),element=this.getElement();if(!container||!element.dom){return this.defaultConstraint;}
var dom=element.dom,containerDom=container.dom,width=dom.offsetWidth,height=dom.offsetHeight,containerWidth=containerDom.offsetWidth,containerHeight=containerDom.offsetHeight;return{min:{x:0,y:0},max:{x:containerWidth-width,y:containerHeight-height}};},getContainer:function(){var container=this.container;if(!container){container=this.getElement().getParent();if(container){this.sizeMonitors.container=new Ext.util.SizeMonitor({element:container,callback:this.doRefresh,scope:this});this.container=container;}}
return container;},detachListeners:function(){this.getElement().un(this.listeners);},isAxisEnabled:function(axis){var direction=this.getDirection();if(axis==='x'){return(direction===this.DIRECTION_BOTH||direction===this.DIRECTION_HORIZONTAL);}
return(direction===this.DIRECTION_BOTH||direction===this.DIRECTION_VERTICAL);},onDragStart:function(e){if(this.getDisabled()){return false;}
var offset=this.offset;this.fireAction('dragstart',[this,e,offset.x,offset.y],this.initDragStart);},initDragStart:function(me,e,offsetX,offsetY){this.dragStartOffset={x:offsetX,y:offsetY};this.isDragging=true;this.getElement().addCls(this.getDraggingCls());},onDrag:function(e){if(!this.isDragging){return;}
var startOffset=this.dragStartOffset;this.fireAction('drag',[this,e,startOffset.x+e.deltaX,startOffset.y+e.deltaY],this.doDrag);},doDrag:function(me,e,offsetX,offsetY){me.setOffset(offsetX,offsetY);},onDragEnd:function(e){if(!this.isDragging){return;}
this.onDrag(e);this.isDragging=false;this.getElement().removeCls(this.getDraggingCls());this.fireEvent('dragend',this,e,this.offset.x,this.offset.y);},setOffset:function(x,y,animation){var currentOffset=this.offset,constraint=this.getConstraint(),minOffset=constraint.min,maxOffset=constraint.max,min=Math.min,max=Math.max;if(this.isAxisEnabled('x')&&typeof x=='number'){x=min(max(x,minOffset.x),maxOffset.x);}
else{x=currentOffset.x;}
if(this.isAxisEnabled('y')&&typeof y=='number'){y=min(max(y,minOffset.y),maxOffset.y);}
else{y=currentOffset.y;}
currentOffset.x=x;currentOffset.y=y;this.getTranslatable().translate(x,y,animation);},getOffset:function(){return this.offset;},refreshConstraint:function(){this.setConstraint(this.currentConstraint);},refreshOffset:function(){var offset=this.offset;this.setOffset(offset.x,offset.y);},doRefresh:function(){this.refreshConstraint();this.getTranslatable().refresh();this.refreshOffset();},refresh:function(){var sizeMonitors=this.sizeMonitors;if(sizeMonitors.element){sizeMonitors.element.refresh();}
if(sizeMonitors.container){sizeMonitors.container.refresh();}
this.doRefresh();},enable:function(){return this.setDisabled(false);},disable:function(){return this.setDisabled(true);},destroy:function(){var sizeMonitors=this.sizeMonitors,translatable=this.getTranslatable();if(sizeMonitors.element){sizeMonitors.element.destroy();}
if(sizeMonitors.container){sizeMonitors.container.destroy();}
var element=this.getElement();if(element&&!element.isDestroyed){element.removeCls(this.getCls());}
this.detachListeners();if(translatable){translatable.destroy();}}},function(){this.override({constructor:function(config){if(config&&config.constrain){Ext.Logger.deprecate("'constrain' config is deprecated, please use 'contraint' instead");config.contraint=config.constrain;delete config.constrain;}
return this.callOverridden(arguments);}});});Ext.define('Ext.behavior.Draggable',{extend:'Ext.behavior.Behavior',requires:['Ext.util.Draggable'],constructor:function(){this.listeners={painted:'onComponentPainted',scope:this};this.callParent(arguments);},onComponentPainted:function(){this.draggable.refresh();},setConfig:function(config){var draggable=this.draggable,component=this.component;if(config){if(!draggable){component.setTranslatable(true);this.draggable=draggable=new Ext.util.Draggable(config);draggable.setTranslatable(component.getTranslatable());draggable.setElement(component.renderElement);draggable.on('destroy','onDraggableDestroy',this);if(component.isPainted()){this.onComponentPainted(component);}
component.on(this.listeners);}
else if(Ext.isObject(config)){draggable.setConfig(config);}}
else if(draggable){draggable.destroy();}
return this;},getDraggable:function(){return this.draggable;},onDraggableDestroy:function(){var component=this.component;delete this.draggable;component.un(this.listeners);},onComponentDestroy:function(){var draggable=this.draggable;if(draggable){draggable.destroy();}}});(function(clsPrefix){Ext.define('Ext.Component',{extend:'Ext.AbstractComponent',alternateClassName:'Ext.lib.Component',mixins:['Ext.mixin.Traversable'],requires:['Ext.ComponentManager','Ext.XTemplate','Ext.dom.Element','Ext.behavior.Translatable','Ext.behavior.Draggable'],xtype:'component',observableType:'component',cachedConfig:{baseCls:null,cls:null,floatingCls:null,hiddenCls:clsPrefix+'item-hidden',ui:null,margin:null,padding:null,border:null,styleHtmlCls:clsPrefix+'html',styleHtmlContent:null},eventedConfig:{left:null,top:null,right:null,bottom:null,width:null,height:null,minWidth:null,minHeight:null,maxWidth:null,maxHeight:null,docked:null,centered:null,hidden:null,disabled:null},config:{style:null,html:null,draggable:null,translatable:null,renderTo:null,zIndex:null,tpl:null,enterAnimation:null,exitAnimation:null,showAnimation:null,hideAnimation:null,tplWriteMode:'overwrite',data:null,disabledCls:clsPrefix+'item-disabled',contentEl:null,itemId:undefined,record:null,plugins:null},listenerOptionsRegex:/^(?:delegate|single|delay|buffer|args|prepend|element)$/,alignmentRegex:/^([a-z]+)-([a-z]+)(\?)?$/,isComponent:true,floating:false,rendered:false,dockPositions:{top:true,right:true,bottom:true,left:true},innerElement:null,element:null,template:[],constructor:function(config){var me=this,currentConfig=me.config,id;me.onInitializedListeners=[];me.initialConfig=config;if(config!==undefined&&'id'in config){id=config.id;}
else if('id'in currentConfig){id=currentConfig.id;}
else{id=me.getId();}
me.id=id;me.setId(id);Ext.ComponentManager.register(me);me.initElement();me.initConfig(me.initialConfig);me.initialize();me.triggerInitialized();if('fullscreen'in me.config){me.fireEvent('fullscreen',me);}
me.fireEvent('initialize',me);},beforeInitConfig:function(config){this.beforeInitialize.apply(this,arguments);},beforeInitialize:Ext.emptyFn,initialize:Ext.emptyFn,getTemplate:function(){return this.template;},getElementConfig:function(){return{reference:'element',children:this.getTemplate()};},triggerInitialized:function(){var listeners=this.onInitializedListeners,ln=listeners.length,listener,i;if(!this.initialized){this.initialized=true;if(ln>0){for(i=0;i<ln;i++){listener=listeners[i];listener.fn.call(listener.scope,this);}
listeners.length=0;}}},onInitialized:function(fn,scope){var listeners=this.onInitializedListeners;if(!scope){scope=this;}
if(this.initialized){fn.call(scope,this);}
else{listeners.push({fn:fn,scope:scope});}},renderTo:function(container,insertBeforeElement){var dom=this.renderElement.dom,containerDom=Ext.getDom(container),insertBeforeChildDom=Ext.getDom(insertBeforeElement);if(containerDom){if(insertBeforeChildDom){containerDom.insertBefore(dom,insertBeforeChildDom);}
else{containerDom.appendChild(dom);}
this.setRendered(Boolean(dom.offsetParent));}},setParent:function(parent){var currentParent=this.parent;if(parent&&currentParent&&currentParent!==parent){currentParent.remove(this,false);}
this.parent=parent;return this;},applyPlugins:function(config){var ln,i,configObj;if(!config){return config;}
config=[].concat(config);for(i=0,ln=config.length;i<ln;i++){configObj=config[i];if(Ext.isObject(configObj)&&configObj.ptype){Ext.Logger.deprecate('Using a ptype is now deprecated, please use type instead',1);configObj.type=configObj.ptype;}
config[i]=Ext.factory(configObj,'Ext.plugin.Plugin',null,'plugin');}
return config;},updatePlugins:function(newPlugins,oldPlugins){var ln,i;if(newPlugins){for(i=0,ln=newPlugins.length;i<ln;i++){newPlugins[i].init(this);}}
if(oldPlugins){for(i=0,ln=oldPlugins.length;i<ln;i++){Ext.destroy(oldPlugins[i]);}}},updateRenderTo:function(newContainer){this.renderTo(newContainer);},updateStyle:function(style){this.element.applyStyles(style);},updateBorder:function(border){this.element.setBorder(border);},updatePadding:function(padding){this.innerElement.setPadding(padding);},updateMargin:function(margin){this.element.setMargin(margin);},updateUi:function(newUi,oldUi){var baseCls=this.getBaseCls();if(baseCls){if(oldUi){this.element.removeCls(oldUi,baseCls);}
if(newUi){this.element.addCls(newUi,baseCls);}}},applyBaseCls:function(baseCls){return baseCls||clsPrefix+this.xtype;},updateBaseCls:function(newBaseCls,oldBaseCls){var me=this,ui=me.getUi();if(newBaseCls){this.element.addCls(newBaseCls);if(ui){this.element.addCls(newBaseCls,null,ui);}}
if(oldBaseCls){this.element.removeCls(oldBaseCls);if(ui){this.element.removeCls(oldBaseCls,null,ui);}}},addCls:function(cls,prefix,suffix){var oldCls=this.getCls(),newCls=(oldCls)?oldCls.slice():[],ln,i,cachedCls;prefix=prefix||'';suffix=suffix||'';if(typeof cls=="string"){cls=[cls];}
ln=cls.length;if(!newCls.length&&prefix===''&&suffix===''){newCls=cls;}else{for(i=0;i<ln;i++){cachedCls=prefix+cls[i]+suffix;if(newCls.indexOf(cachedCls)==-1){newCls.push(cachedCls);}}}
this.setCls(newCls);},removeCls:function(cls,prefix,suffix){var oldCls=this.getCls(),newCls=(oldCls)?oldCls.slice():[],ln,i;prefix=prefix||'';suffix=suffix||'';if(typeof cls=="string"){newCls=Ext.Array.remove(newCls,prefix+cls+suffix);}else{ln=cls.length;for(i=0;i<ln;i++){newCls=Ext.Array.remove(newCls,prefix+cls[i]+suffix);}}
this.setCls(newCls);},replaceCls:function(oldCls,newCls,prefix,suffix){var cls=this.getCls(),array=(cls)?cls.slice():[],ln,i,cachedCls;prefix=prefix||'';suffix=suffix||'';if(typeof oldCls=="string"){array=Ext.Array.remove(array,prefix+oldCls+suffix);}else if(oldCls){ln=oldCls.length;for(i=0;i<ln;i++){array=Ext.Array.remove(array,prefix+oldCls[i]+suffix);}}
if(typeof newCls=="string"){array.push(prefix+newCls+suffix);}else if(newCls){ln=newCls.length;if(!array.length&&prefix===''&&suffix===''){array=newCls;}else{for(i=0;i<ln;i++){cachedCls=prefix+newCls[i]+suffix;if(array.indexOf(cachedCls)==-1){array.push(cachedCls);}}}}
this.setCls(array);},applyCls:function(cls){if(typeof cls=="string"){cls=[cls];}
if(!cls||!cls.length){cls=null;}
return cls;},updateCls:function(newCls,oldCls){this.element.replaceCls(oldCls,newCls);},updateStyleHtmlCls:function(newHtmlCls,oldHtmlCls){var innerHtmlElement=this.innerHtmlElement,innerElement=this.innerElement;if(this.getStyleHtmlContent()&&oldHtmlCls){if(innerHtmlElement){innerHtmlElement.replaceCls(oldHtmlCls,newHtmlCls);}else{innerElement.replaceCls(oldHtmlCls,newHtmlCls);}}},applyStyleHtmlContent:function(config){return Boolean(config);},updateStyleHtmlContent:function(styleHtmlContent){var htmlCls=this.getStyleHtmlCls(),innerElement=this.innerElement,innerHtmlElement=this.innerHtmlElement;if(styleHtmlContent){if(innerHtmlElement){innerHtmlElement.addCls(htmlCls);}else{innerElement.addCls(htmlCls);}}else{if(innerHtmlElement){innerHtmlElement.removeCls(htmlCls);}else{innerElement.addCls(htmlCls);}}},applyContentEl:function(contentEl){if(contentEl){return Ext.get(contentEl);}},updateContentEl:function(newContentEl,oldContentEl){if(oldContentEl){oldContentEl.hide();Ext.getBody().append(oldContentEl);}
if(newContentEl){this.setHtml(newContentEl.dom);newContentEl.show();}},getSize:function(){return{width:this.getWidth(),height:this.getHeight()};},isCentered:function(){return Boolean(this.getCentered());},isFloating:function(){return this.floating;},isDocked:function(){return Boolean(this.getDocked());},isInnerItem:function(){var me=this;return!me.isCentered()&&!me.isFloating()&&!me.isDocked();},filterPositionValue:function(value){if(value===''||value==='auto'){value=null;}
return value;},applyTop:function(top){return this.filterPositionValue(top);},applyRight:function(right){return this.filterPositionValue(right);},applyBottom:function(bottom){return this.filterPositionValue(bottom);},applyLeft:function(left){return this.filterPositionValue(left);},doSetTop:function(top){this.updateFloating();this.element.setTop(top);},doSetRight:function(right){this.updateFloating();this.element.setRight(right);},doSetBottom:function(bottom){this.updateFloating();this.element.setBottom(bottom);},doSetLeft:function(left){this.updateFloating();this.element.setLeft(left);},doSetWidth:function(width){this.element.setWidth(width);},doSetHeight:function(height){this.element.setHeight(height);},doSetMinWidth:function(width){this.element.setMinWidth(width);},doSetMinHeight:function(height){this.element.setMinHeight(height);},doSetMaxWidth:function(width){this.element.setMaxWidth(width);},doSetMaxHeight:function(height){this.element.setMaxHeight(height);},applyCentered:function(centered){centered=Boolean(centered);if(centered){if(this.isFloating()){this.resetFloating();}
if(this.isDocked()){this.setDocked(false);}}
return centered;},doSetCentered:Ext.emptyFn,applyDocked:function(docked){if(docked){if(!this.dockPositions[docked]){Ext.Logger.error("Invalid docking position of '"+docked+"', must be either 'top', 'right', 'bottom', "+"'left' or `null` (for no docking)",this);return;}
if(this.isFloating()){this.resetFloating();}
if(this.isCentered()){this.setCentered(false);}}
return docked;},doSetDocked:Ext.emptyFn,resetFloating:function(){this.setTop(null);this.setRight(null);this.setBottom(null);this.setLeft(null);},updateFloating:function(){var floating=true,floatingCls=this.getFloatingCls();if(this.getTop()===null&&this.getBottom()===null&&this.getRight()===null&&this.getLeft()===null){floating=false;}
if(floating!==this.floating){if(floating){if(this.isCentered()){this.setCentered(false);}
if(this.isDocked()){this.setDocked(false);}
if(floatingCls){this.addCls(floatingCls);}}else if(floatingCls){this.removeCls(floatingCls);}
this.floating=floating;this.fireEvent('floatingchange',this,floating);}},updateFloatingCls:function(newFloatingCls,oldFloatingCls){if(this.isFloating()){this.replaceCls(oldFloatingCls,newFloatingCls);}},applyDisabled:function(disabled){return Boolean(disabled);},doSetDisabled:function(disabled){this.element[disabled?'addCls':'removeCls'](this.getDisabledCls());},updateDisabledCls:function(newDisabledCls,oldDisabledCls){if(this.isDisabled()){this.element.replaceCls(oldDisabledCls,newDisabledCls);}},disable:function(){this.setDisabled(true);},enable:function(){this.setDisabled(false);},isDisabled:function(){return this.getDisabled();},applyZIndex:function(zIndex){if(zIndex!==null){zIndex=Number(zIndex);if(isNaN(zIndex)){zIndex=null;}}
return zIndex;},updateZIndex:function(zIndex){var domStyle=this.element.dom.style;if(zIndex!==null){domStyle.setProperty('z-index',zIndex,'important');}
else{domStyle.removeProperty('z-index');}},getInnerHtmlElement:function(){var innerHtmlElement=this.innerHtmlElement,styleHtmlCls=this.getStyleHtmlCls();if(!innerHtmlElement||!innerHtmlElement.dom||!innerHtmlElement.dom.parentNode){this.innerHtmlElement=innerHtmlElement=this.innerElement.createChild({cls:'x-innerhtml '});if(this.getStyleHtmlContent()){this.innerHtmlElement.addCls(styleHtmlCls);this.innerElement.removeCls(styleHtmlCls);}}
return innerHtmlElement;},updateHtml:function(html){var innerHtmlElement=this.getInnerHtmlElement();if(Ext.isElement(html)){innerHtmlElement.setHtml('');innerHtmlElement.append(html);}
else{innerHtmlElement.setHtml(html);}},applyHidden:function(hidden){return Boolean(hidden);},doSetHidden:function(hidden){var element=this.renderElement;if(element.isDestroyed){return;}
if(hidden){element.hide();}
else{element.show();}
if(this.element){this.element[hidden?'addCls':'removeCls'](this.getHiddenCls());}
this.fireEvent(hidden?'hide':'show',this);},updateHiddenCls:function(newHiddenCls,oldHiddenCls){if(this.isHidden()){this.element.replaceCls(oldHiddenCls,newHiddenCls);}},isHidden:function(){return this.getHidden();},hide:function(animation){if(!this.getHidden()){if(animation===undefined||(animation&&animation.isComponent)){animation=this.getHideAnimation();}
if(animation){if(animation===true){animation='fadeOut';}
this.onBefore({hiddenchange:'animateFn',scope:this,single:true,args:[animation]});}
this.setHidden(true);}
return this;},show:function(animation){var hidden=this.getHidden();if(hidden||hidden===null){if(animation===undefined||(animation&&!animation.isComponent)){animation=this.getShowAnimation();}
if(animation){if(animation===true){animation='fadeIn';}
this.onBefore({hiddenchange:'animateFn',scope:this,single:true,args:[animation]});}
this.setHidden(false);}
return this;},animateFn:function(animation,component,newState,oldState,options,controller){if(animation){var anim=new Ext.fx.Animation(animation);anim.setElement(component.element);if(newState){anim.setOnEnd(function(){controller.resume();});controller.pause();}
Ext.Animator.run(anim);}},setVisibility:function(isVisible){this.renderElement.setVisibility(isVisible);},isRendered:function(){return this.rendered;},isPainted:function(){return this.renderElement.isPainted();},applyTpl:function(config){return(Ext.isObject(config)&&config.isTemplate)?config:new Ext.XTemplate(config);},applyData:function(data){if(Ext.isObject(data)){return Ext.apply({},data);}
return data;},updateData:function(newData){var me=this;if(newData){var tpl=me.getTpl(),tplWriteMode=me.getTplWriteMode();if(tpl){tpl[tplWriteMode](me.getInnerHtmlElement(),newData);}
this.fireEvent('updatedata',me,newData);}},applyRecord:function(config){if(config&&Ext.isObject(config)&&config.isModel){return config;}
return null;},updateRecord:function(newRecord,oldRecord){var me=this;if(oldRecord){oldRecord.unjoin(me);}
if(!newRecord){me.updateData('');}
else{newRecord.join(me);me.updateData(newRecord.getData(true));}},afterEdit:function(){this.updateRecord(this.getRecord());},afterErase:function(){this.setRecord(null);},applyItemId:function(itemId){return itemId||this.getId();},isXType:function(xtype,shallow){if(shallow){return this.xtypes.indexOf(xtype)!=-1;}
return Boolean(this.xtypesMap[xtype]);},getXTypes:function(){return this.xtypesChain.join('/');},getDraggableBehavior:function(){var behavior=this.draggableBehavior;if(!behavior){behavior=this.draggableBehavior=new Ext.behavior.Draggable(this);}
return behavior;},applyDraggable:function(config){this.getDraggableBehavior().setConfig(config);},getDraggable:function(){return this.getDraggableBehavior().getDraggable();},getTranslatableBehavior:function(){var behavior=this.translatableBehavior;if(!behavior){behavior=this.translatableBehavior=new Ext.behavior.Translatable(this);}
return behavior;},applyTranslatable:function(config){this.getTranslatableBehavior().setConfig(config);},getTranslatable:function(){return this.getTranslatableBehavior().getTranslatable();},translateAxis:function(axis,value,animation){var x,y;if(axis==='x'){x=value;}
else{y=value;}
return this.translate(x,y,animation);},translate:function(){var translatable=this.getTranslatable();if(!translatable){this.setTranslatable(true);translatable=this.getTranslatable();}
translatable.translate.apply(translatable,arguments);},setRendered:function(rendered){var wasRendered=this.rendered;if(rendered!==wasRendered){this.rendered=rendered;return true;}
return false;},setSize:function(width,height){if(width!=undefined){this.setWidth(width);}
if(height!=undefined){this.setHeight(height);}},doAddListener:function(name,fn,scope,options,order){if(options&&'element'in options){if(this.referenceList.indexOf(options.element)===-1){Ext.Logger.error("Adding event listener with an invalid element reference of '"+options.element+"' for this component. Available values are: '"+this.referenceList.join("', '")+"'",this);}
this[options.element].doAddListener(name,fn,scope||this,options,order);}
return this.callParent(arguments);},doRemoveListener:function(name,fn,scope,options,order){if(options&&'element'in options){if(this.referenceList.indexOf(options.element)===-1){Ext.Logger.error("Removing event listener with an invalid element reference of '"+options.element+"' for this component. Available values are: '"+this.referenceList.join('", "')+"'",this);}
this[options.element].doRemoveListener(name,fn,scope||this,options,order);}
return this.callParent(arguments);},showBy:function(component,alignment){var args=Ext.Array.from(arguments);var viewport=Ext.Viewport,parent=this.getParent();this.setVisibility(false);if(parent!==viewport){viewport.add(this);}
this.show();this.on('erased','onShowByErased',this,{single:true});viewport.on('resize','refreshShowBy',this,{args:[component,alignment]});this.currentShowByArgs=args;this.alignTo(component,alignment);this.setVisibility(true);},refreshShowBy:function(component,alignment){this.alignTo(component,alignment);},onShowByErased:function(){Ext.Viewport.un('resize','refreshShowBy',this);},alignTo:function(component,alignment){var alignToElement=component.isComponent?component.renderElement:component,element=this.renderElement,alignToBox=alignToElement.getPageBox(),constrainBox=this.getParent().element.getPageBox(),box=element.getPageBox(),alignToHeight=alignToBox.height,alignToWidth=alignToBox.width,height=box.height,width=box.width;constrainBox.bottom-=5;constrainBox.height-=10;constrainBox.left+=5;constrainBox.right-=5;constrainBox.top+=5;constrainBox.width-=10;if(!alignment||alignment==='auto'){if(constrainBox.bottom-alignToBox.bottom<height){if(alignToBox.top-constrainBox.top<height){if(alignToBox.left-constrainBox.left<width){alignment='cl-cr?';}
else{alignment='cr-cl?';}}
else{alignment='bc-tc?';}}
else{alignment='tc-bc?';}}
var matches=alignment.match(this.alignmentRegex);if(!matches){Ext.Logger.error("Invalid alignment value of '"+alignment+"'");}
var from=matches[1].split(''),to=matches[2].split(''),constrained=(matches[3]==='?'),fromVertical=from[0],fromHorizontal=from[1]||fromVertical,toVertical=to[0],toHorizontal=to[1]||toVertical,top=alignToBox.top,left=alignToBox.left,halfAlignHeight=alignToHeight/2,halfAlignWidth=alignToWidth/2,halfWidth=width/2,halfHeight=height/2,maxLeft,maxTop;switch(fromVertical){case't':switch(toVertical){case'c':top+=halfAlignHeight;break;case'b':top+=alignToHeight;}
break;case'b':switch(toVertical){case'c':top-=(height-halfAlignHeight);break;case't':top-=height;break;case'b':top-=height-alignToHeight;}
break;case'c':switch(toVertical){case't':top-=halfHeight;break;case'c':top-=(halfHeight-halfAlignHeight);break;case'b':top-=(halfHeight-alignToHeight);}
break;}
switch(fromHorizontal){case'l':switch(toHorizontal){case'c':left+=halfAlignHeight;break;case'r':left+=alignToWidth;}
break;case'r':switch(toHorizontal){case'r':left-=(width-alignToWidth);break;case'c':left-=(width-halfWidth);break;case'l':left-=width;}
break;case'c':switch(toHorizontal){case'l':left-=halfWidth;break;case'c':left-=(halfWidth-halfAlignWidth);break;case'r':left-=(halfWidth-alignToWidth);}
break;}
if(constrained){maxLeft=(constrainBox.left+constrainBox.width)-width;maxTop=(constrainBox.top+constrainBox.height)-height;left=Math.max(constrainBox.left,Math.min(maxLeft,left));top=Math.max(constrainBox.top,Math.min(maxTop,top));}
this.setLeft(left);this.setTop(top);},up:function(selector){var result=this.parent;if(selector){for(;result;result=result.parent){if(Ext.ComponentQuery.is(result,selector)){return result;}}}
return result;},getBubbleTarget:function(){return this.getParent();},destroy:function(){this.destroy=Ext.emptyFn;var parent=this.getParent(),referenceList=this.referenceList,i,ln,reference;if(parent){parent.remove(this,false);}
for(i=0,ln=referenceList.length;i<ln;i++){reference=referenceList[i];this[reference].destroy();delete this[reference];}
this.setRecord(null);Ext.destroy(this.innerHtmlElement,this.getTranslatable());Ext.ComponentManager.unregister(this);this.callParent();},onClassExtended:function(cls,data){var Component=this,defaultConfig=Component.prototype.config,config=data.config||{},key;for(key in defaultConfig){if(key in data){config[key]=data[key];delete data[key];Ext.Logger.deprecate(key+' is deprecated as a property directly on the Component. '+'Please put it inside the config object, and retrieve it using "this.config.'+key+'"');}}
data.config=config;}},function(){var emptyFn=Ext.emptyFn;this.override({constructor:function(config){var name;if(config){if(config.enabled){Ext.Logger.deprecate("'enabled' config is deprecated, please use 'disabled' config instead",this);config.disabled=!config.enabled;}
if((config.scroll||this.config.scroll||this.scrollable||this.config.scrollable)&&!this.isContainer){Ext.Logger.deprecate("You are no longer able to scroll a component. Please use a Ext.Container instead.",this);delete config.scrollable;delete config.scroll;}
if((config.hideOnMaskTap||this.config.hideOnMaskTap)&&!this.isContainer){Ext.Logger.deprecate("You are no longer able use hideOnMaskTap on a component. Please use a Ext.Container instead.",this);delete config.hideOnMaskTap;}
if((config.modal||this.config.modal)&&!this.isContainer){Ext.Logger.deprecate("You are no longer able use modal on a component. Please use a Ext.Container instead.",this);delete config.modal;}
if(config.dock){Ext.Logger.deprecate("'dock' config for docked items is deprecated, please use 'docked' instead");config.docked=config.dock;delete config.dock;}
if(config.enterAnimation){Ext.Logger.deprecate("'enterAnimation' config for Components is deprecated, please use 'showAnimation' instead");config.showAnimation=config.enterAnimation;delete config.enterAnimation;}
if(config.exitAnimation){Ext.Logger.deprecate("'exitAnimation' config for Components is deprecated, please use 'hideAnimation' instead");config.hideAnimation=config.exitAnimation;delete config.exitAnimation;}
if(config.componentCls){Ext.Logger.deprecate("'componentCls' config is deprecated, please use 'cls' config instead",this);config.cls=config.componentCls;}
if(config.floating){Ext.Logger.deprecate("'floating' config is deprecated, please set 'left', 'right', "+"'top' or 'bottom' config instead",this);config.left=config.left||0;}
for(name in config){if(config.hasOwnProperty(name)&&name!=='xtype'&&name!=='xclass'&&!this.hasConfig(name)){this[name]=config[name];}}
if(config.layoutOnOrientationChange){Ext.Logger.deprecate("'layoutOnOrientationChange' has been fully removed and no longer used");delete config.layoutOnOrientationChange;}
if(config.monitorOrientation){Ext.Logger.deprecate("'monitorOrientation' has been removed. If you need to monitor the orientaiton, please use the 'resize' event.");delete config.monitorOrientation;}
if(config.stopMaskTapEvent){Ext.Logger.deprecate("'stopMaskTapEvent' has been removed.");delete config.stopMaskTapEvent;}}
this.callParent(arguments);if(this.onRender!==emptyFn){Ext.Logger.deprecate("onRender() is deprecated, please put your code inside initialize() instead",this);this.onRender();}
if(this.afterRender!==emptyFn){Ext.Logger.deprecate("afterRender() is deprecated, please put your code inside initialize() instead",this);this.afterRender();}
if(this.initEvents!==emptyFn){Ext.Logger.deprecate("initEvents() is deprecated, please put your code inside initialize() instead",this);this.initEvents();}
if(this.initComponent!==emptyFn){Ext.Logger.deprecate("initComponent() is deprecated, please put your code inside initialize() instead",this);this.initComponent();}
if(this.setOrientation!==emptyFn){Ext.Logger.deprecate("setOrientation() is deprecated",this);this.setOrientation();}},onRender:emptyFn,afterRender:emptyFn,initEvents:emptyFn,initComponent:emptyFn,setOrientation:emptyFn,show:function(){if(this.renderElement.dom){var containerDom=this.renderElement.dom.parentNode;if(containerDom&&containerDom.nodeType==11){Ext.Logger.deprecate("Call show() on a component that doesn't currently belong to any container. "+"Please add it to the the Viewport first, i.e: Ext.Viewport.add(component);",this);Ext.Viewport.add(this);}}
return this.callParent(arguments);},doAddListener:function(name,fn,scope,options,order){switch(name){case'render':Ext.Logger.warn("The render event on Components is deprecated. Please use the painted event. "+"Please refer to: http://bit.ly/xgv3K1 for more details.",this);return this;break;}
return this.callParent(arguments);},addListener:function(options){if(arguments.length===1&&Ext.isObject(options)&&(('el'in options)||('body'in options))){Ext.Logger.error("Adding component element listeners using the old format is no longer supported. "+"Please refer to: http://bit.ly/xHCyfa for more details.",this);}
return this.callParent(arguments);},getEl:function(){Ext.Logger.deprecate("getEl() is deprecated, please access the Component's element from "+"the 'element' property instead",this);return this.renderElement;},setFloating:function(floating){var isFloating=this.isFloating();if(floating&&!isFloating){this.setTop(0);}else if(isFloating){this.resetFloating();}},setScrollable:function(){Ext.Logger.deprecate("Ext.Component cannot be scrollable. Please use Ext.Container#setScrollable on a Ext.Container.",this);return false;}});Ext.deprecateClassMembers(this,{el:'element',body:'element',outer:'renderElement',ownerCt:'parent',update:'setHtml'});});})(Ext.baseCSSPrefix);Ext.define('Ext.Button',{extend:'Ext.Component',xtype:'button',cachedConfig:{pressedCls:Ext.baseCSSPrefix+'button-pressing',badgeCls:Ext.baseCSSPrefix+'badge',hasBadgeCls:Ext.baseCSSPrefix+'hasbadge',labelCls:Ext.baseCSSPrefix+'button-label',iconMaskCls:Ext.baseCSSPrefix+'icon-mask'},config:{badgeText:null,text:null,iconCls:null,icon:null,iconAlign:'left',pressedDelay:0,iconMask:null,handler:null,scope:null,autoEvent:null,ui:'normal',baseCls:Ext.baseCSSPrefix+'button'},template:[{tag:'span',reference:'badgeElement',hidden:true},{tag:'span',className:Ext.baseCSSPrefix+'button-icon',reference:'iconElement',hidden:true},{tag:'span',reference:'textElement',hidden:true}],initialize:function(){this.callParent();this.element.on({scope:this,tap:'onTap',touchstart:'onPress',touchend:'onRelease'});},updateBadgeText:function(badgeText){var element=this.element,badgeElement=this.badgeElement;if(badgeText){badgeElement.show();badgeElement.setText(badgeText);}
else{badgeElement.hide();}
element[(badgeText)?'addCls':'removeCls'](this.getHasBadgeCls());},updateText:function(text){var textElement=this.textElement;if(text){textElement.show();textElement.setHtml(text);}
else{textElement.hide();}},updateHtml:function(html){var textElement=this.textElement;if(html){textElement.show();textElement.setHtml(html);}
else{textElement.hide();}},updateBadgeCls:function(badgeCls,oldBadgeCls){this.badgeElement.replaceCls(oldBadgeCls,badgeCls);},updateHasBadgeCls:function(hasBadgeCls,oldHasBadgeCls){var element=this.element;if(element.hasCls(oldHasBadgeCls)){element.replaceCls(oldHasBadgeCls,hasBadgeCls);}},updateLabelCls:function(labelCls,oldLabelCls){this.textElement.replaceCls(oldLabelCls,labelCls);},updatePressedCls:function(pressedCls,oldPressedCls){var element=this.element;if(element.hasCls(oldPressedCls)){element.replaceCls(oldPressedCls,pressedCls);}},updateIcon:function(icon){var me=this,element=me.iconElement;if(icon){me.showIconElement();element.setStyle('background-image',icon?'url('+icon+')':'');me.refreshIconAlign();me.refreshIconMask();}
else{me.hideIconElement();me.setIconAlign(false);}},updateIconCls:function(iconCls,oldIconCls){var me=this,element=me.iconElement;if(iconCls){me.showIconElement();element.replaceCls(oldIconCls,iconCls);me.refreshIconAlign();me.refreshIconMask();}
else{me.hideIconElement();me.setIconAlign(false);}},updateIconAlign:function(alignment,oldAlignment){var element=this.element,baseCls=Ext.baseCSSPrefix+'iconalign-';if(!this.getText()){alignment="center";}
element.removeCls(baseCls+"center");element.removeCls(baseCls+oldAlignment);if(this.getIcon()||this.getIconCls()){element.addCls(baseCls+alignment);}},refreshIconAlign:function(){this.updateIconAlign(this.getIconAlign());},updateIconMaskCls:function(iconMaskCls,oldIconMaskCls){var element=this.iconElement;if(this.getIconMask()){element.replaceCls(oldIconMaskCls,iconMaskCls);}},updateIconMask:function(iconMask){this.iconElement[iconMask?"addCls":"removeCls"](this.getIconMaskCls());},refreshIconMask:function(){this.updateIconMask(this.getIconMask());},applyAutoEvent:function(autoEvent){var me=this;if(typeof autoEvent=='string'){autoEvent={name:autoEvent,scope:me.scope||me};}
return autoEvent;},updateAutoEvent:function(autoEvent){var name=autoEvent.name,scope=autoEvent.scope;this.setHandler(function(){scope.fireEvent(name,scope,this);});this.setScope(scope);},hideIconElement:function(){this.iconElement.hide();},showIconElement:function(){this.iconElement.show();},applyUi:function(config){if(config&&Ext.isString(config)){var array=config.split('-');if(array&&(array[1]=="back"||array[1]=="forward")){return array;}}
return config;},getUi:function(){var ui=this._ui;if(Ext.isArray(ui)){return ui.join('-');}
return ui;},applyPressedDelay:function(delay){if(Ext.isNumber(delay)){return delay;}
return(delay)?100:0;},onPress:function(){var element=this.element,pressedDelay=this.getPressedDelay(),pressedCls=this.getPressedCls();if(!this.getDisabled()){this.isPressed=true;if(this.hasOwnProperty('releasedTimeout')){clearTimeout(this.releasedTimeout);delete this.releasedTimeout;}
if(pressedDelay>0){this.pressedTimeout=setTimeout(function(){if(element){element.addCls(pressedCls);}},pressedDelay);}
else{element.addCls(pressedCls);}}},onRelease:function(e){this.fireAction('release',[this,e],'doRelease');},doRelease:function(me,e){if(!me.isPressed){return;}
me.isPressed=false;if(me.hasOwnProperty('pressedTimeout')){clearTimeout(me.pressedTimeout);delete me.pressedTimeout;}
me.releasedTimeout=setTimeout(function(){if(me&&me.element){me.element.removeCls(me.getPressedCls());}},10);},onTap:function(e){if(this.getDisabled()){return false;}
this.fireAction('tap',[this,e],'doTap');},doTap:function(me,e){var handler=me.getHandler(),scope=me.getScope()||me;if(!handler){return;}
if(typeof handler=='string'){handler=scope[handler];}
e.preventDefault();handler.apply(scope,arguments);}},function(){Ext.deprecateClassMethod(this,'setBadge','setBadgeText');Ext.deprecateClassMethod(this,'setIconClass','setIconCls');this.override({constructor:function(config){if(config){if(config.hasOwnProperty('badge')){Ext.Logger.deprecate("'badge' config is deprecated, please use 'badgeText' config instead",this);config.badgeText=config.badge;}}
this.callParent([config]);}});});Ext.define('Ext.Title',{extend:'Ext.Component',xtype:'title',config:{baseCls:'x-title',title:''},updateTitle:function(newTitle){this.setHtml(newTitle);}});Ext.define('Ext.Spacer',{extend:'Ext.Component',alias:'widget.spacer',config:{},constructor:function(config){config=config||{};if(!config.width){config.flex=1;}
this.callParent([config]);}});Ext.define('Ext.Mask',{extend:'Ext.Component',xtype:'mask',config:{baseCls:Ext.baseCSSPrefix+'mask',transparent:false,top:0,left:0,right:0,bottom:0},initialize:function(){this.callParent();this.on({painted:'onPainted',erased:'onErased'})},onPainted:function(){this.element.on('*','onEvent',this);},onErased:function(){this.element.un('*','onEvent',this);},onEvent:function(e){var controller=arguments[arguments.length-1];if(controller.info.eventName==='tap'){this.fireEvent('tap',this,e);return false;}
if(e&&e.stopEvent){e.stopEvent();}
return false;},updateTransparent:function(newTransparent){this[newTransparent?'addCls':'removeCls'](this.getBaseCls()+'-transparent');}});Ext.define('Ext.Decorator',{extend:'Ext.Component',isDecorator:true,config:{component:{}},statics:{generateProxySetter:function(name){return function(value){var component=this.getComponent();component[name].call(component,value);return this;}},generateProxyGetter:function(name){return function(){var component=this.getComponent();return component[name].call(component);}}},onClassExtended:function(Class,members){if(!members.hasOwnProperty('proxyConfig')){return;}
var ExtClass=Ext.Class,proxyConfig=members.proxyConfig,config=members.config;members.config=(config)?Ext.applyIf(config,proxyConfig):proxyConfig;var name,nameMap,setName,getName;for(name in proxyConfig){if(proxyConfig.hasOwnProperty(name)){nameMap=ExtClass.getConfigNameMap(name);setName=nameMap.set;getName=nameMap.get;members[setName]=this.generateProxySetter(setName);members[getName]=this.generateProxyGetter(getName);}}},applyComponent:function(config){return Ext.factory(config,Ext.Component);},updateComponent:function(newComponent,oldComponent){if(oldComponent){if(this.isRendered()&&oldComponent.setRendered(false)){oldComponent.fireAction('renderedchange',[this,oldComponent,false],'doUnsetComponent',this,{args:[oldComponent]});}
else{this.doUnsetComponent(oldComponent);}}
if(newComponent){if(this.isRendered()&&newComponent.setRendered(true)){newComponent.fireAction('renderedchange',[this,newComponent,true],'doSetComponent',this,{args:[newComponent]});}
else{this.doSetComponent(newComponent);}}},doUnsetComponent:function(component){if(component.renderElement.dom){this.innerElement.dom.removeChild(component.renderElement.dom);}},doSetComponent:function(component){if(component.renderElement.dom){this.innerElement.dom.appendChild(component.renderElement.dom);}},setRendered:function(rendered){var component;if(this.callParent(arguments)){component=this.getComponent();if(component){component.setRendered(rendered);}
return true;}
return false;},setDisabled:function(disabled){this.callParent(arguments);this.getComponent().setDisabled(disabled);},destroy:function(){Ext.destroy(this.getComponent());this.callParent();}});Ext.define('Ext.field.Input',{extend:'Ext.Component',xtype:'input',tag:'input',cachedConfig:{cls:Ext.baseCSSPrefix+'form-field',focusCls:Ext.baseCSSPrefix+'field-focus',maskCls:Ext.baseCSSPrefix+'field-mask',useMask:'auto',type:'text',checked:false},config:{baseCls:Ext.baseCSSPrefix+'field-input',name:null,value:null,isFocused:false,tabIndex:null,placeHolder:null,minValue:null,maxValue:null,stepValue:null,maxLength:null,autoComplete:null,autoCapitalize:null,autoCorrect:null,readOnly:null,maxRows:null,startValue:false},getTemplate:function(){var items=[{reference:'input',tag:this.tag},{reference:'clearIcon',cls:'x-clear-icon'}];items.push({reference:'mask',classList:[this.config.maskCls]});return items;},initElement:function(){var me=this;me.callParent();me.input.on({scope:me,keyup:'onKeyUp',focus:'onFocus',blur:'onBlur',paste:'onPaste'});me.mask.on({tap:'onMaskTap',scope:me});if(me.clearIcon){me.clearIcon.on({tap:'onClearIconTap',scope:me});}},applyUseMask:function(useMask){if(useMask==='auto'){useMask=Ext.os.is.iOS&&Ext.os.version.lt('5');}
return Boolean(useMask);},updateUseMask:function(newUseMask){this.mask[newUseMask?'show':'hide']();},updateFieldAttribute:function(attribute,newValue){var input=this.input;if(newValue){input.dom.setAttribute(attribute,newValue);}else{input.dom.removeAttribute(attribute);}},updateCls:function(newCls,oldCls){this.input.addCls(Ext.baseCSSPrefix+'input-el');this.input.replaceCls(oldCls,newCls);},updateType:function(newType,oldType){var prefix=Ext.baseCSSPrefix+'input-';this.input.replaceCls(prefix+oldType,prefix+newType);this.updateFieldAttribute('type',newType);},updateName:function(newName){this.updateFieldAttribute('name',newName);},getValue:function(){var input=this.input;if(input){this._value=input.dom.value;}
return this._value;},applyValue:function(value){return(Ext.isEmpty(value))?'':value;},updateValue:function(newValue){var input=this.input;if(input){input.dom.value=newValue;}},setValue:function(newValue){this.updateValue(this.applyValue(newValue));return this;},applyTabIndex:function(tabIndex){if(tabIndex!==null&&typeof tabIndex!='number'){throw new Error("Ext.field.Field: [applyTabIndex] trying to pass a value which is not a number");}
return tabIndex;},updateTabIndex:function(newTabIndex){this.updateFieldAttribute('tabIndex',newTabIndex);},testAutoFn:function(value){return[true,'on'].indexOf(value)!==-1;},applyMaxLength:function(maxLength){if(maxLength!==null&&typeof maxLength!='number'){throw new Error("Ext.field.Text: [applyMaxLength] trying to pass a value which is not a number");}
return maxLength;},updateMaxLength:function(newMaxLength){this.updateFieldAttribute('maxlength',newMaxLength);},updatePlaceHolder:function(newPlaceHolder){this.updateFieldAttribute('placeholder',newPlaceHolder);},applyAutoComplete:function(autoComplete){return this.testAutoFn(autoComplete);},updateAutoComplete:function(newAutoComplete){var value=newAutoComplete?'on':'off';this.updateFieldAttribute('autocomplete',value);},applyAutoCapitalize:function(autoCapitalize){return this.testAutoFn(autoCapitalize);},updateAutoCapitalize:function(newAutoCapitalize){var value=newAutoCapitalize?'on':'off';this.updateFieldAttribute('autocapitalize',value);},applyAutoCorrect:function(autoCorrect){return this.testAutoFn(autoCorrect);},updateAutoCorrect:function(newAutoCorrect){var value=newAutoCorrect?'on':'off';this.updateFieldAttribute('autocorrect',value);},updateMinValue:function(newMinValue){this.updateFieldAttribute('min',newMinValue);},updateMaxValue:function(newMaxValue){this.updateFieldAttribute('max',newMaxValue);},updateStepValue:function(newStepValue){this.updateFieldAttribute('step',newStepValue);},checkedRe:/^(true|1|on)/i,getChecked:function(){var el=this.input,checked;if(el){checked=el.dom.checked;this._checked=checked;}
return checked;},applyChecked:function(checked){return!!this.checkedRe.test(String(checked));},setChecked:function(newChecked){this.updateChecked(this.applyChecked(newChecked));this._checked=newChecked;},updateChecked:function(newChecked){this.input.dom.checked=newChecked;},updateReadOnly:function(readOnly){this.updateFieldAttribute('readonly',readOnly);},applyMaxRows:function(maxRows){if(maxRows!==null&&typeof maxRows!=='number'){throw new Error("Ext.field.Input: [applyMaxRows] trying to pass a value which is not a number");}
return maxRows;},updateMaxRows:function(newRows){this.updateFieldAttribute('rows',newRows);},doSetDisabled:function(disabled){this.callParent(arguments);this.input.dom.disabled=disabled;if(!disabled){this.blur();}},isDirty:function(){if(this.getDisabled()){return false;}
return String(this.getValue())!==String(this.originalValue);},reset:function(){this.setValue(this.originalValue);},onMaskTap:function(e){this.fireAction('masktap',[this,e],'doMaskTap');},doMaskTap:function(me,e){if(me.getDisabled()){return false;}
me.maskCorrectionTimer=Ext.defer(me.showMask,1000,me);me.hideMask();},showMask:function(e){if(this.mask){this.mask.setStyle('display','block');}},hideMask:function(e){if(this.mask){this.mask.setStyle('display','none');}},focus:function(){var me=this,el=me.input;if(el&&el.dom.focus){el.dom.focus();}
return me;},blur:function(){var me=this,el=this.input;if(el&&el.dom.blur){el.dom.blur();}
return me;},select:function(){var me=this,el=me.input;if(el&&el.dom.setSelectionRange){el.dom.setSelectionRange(0,9999);}
return me;},onFocus:function(e){this.fireAction('focus',[e],'doFocus');},doFocus:function(e){var me=this;if(me.mask){if(me.maskCorrectionTimer){clearTimeout(me.maskCorrectionTimer);}
me.hideMask();}
if(!me.getIsFocused()){me.setIsFocused(true);me.setStartValue(me.getValue());}},onBlur:function(e){this.fireAction('blur',[e],'doBlur');},doBlur:function(e){var me=this,value=me.getValue(),startValue=me.getStartValue();me.setIsFocused(false);if(String(value)!=String(startValue)){me.onChange(me,value,startValue);}
me.showMask();},onClearIconTap:function(e){var oldValue=this.getValue(),newValue;this.fireEvent('clearicontap',this,e);newValue=this.getValue();if(String(newValue)!=String(oldValue)){this.onChange(this,newValue,oldValue);}
if(Ext.os.is.Android){this.focus();}},onClick:function(e){this.fireEvent('click',e);},onChange:function(me,value,startValue){this.fireEvent('change',me,value,startValue);},onKeyUp:function(e){this.fireEvent('keyup',e);},onPaste:function(e){this.fireEvent('paste',e);},onMouseDown:function(e){this.fireEvent('mousedown',e);}});Ext.define('Ext.field.Field',{extend:'Ext.Decorator',alternateClassName:'Ext.form.Field',xtype:'field',requires:['Ext.field.Input'],isField:true,isFormField:true,config:{baseCls:Ext.baseCSSPrefix+'field',label:null,labelAlign:'left',labelWidth:'30%',clearIcon:null,required:false,inputType:null,name:null,value:null,tabIndex:null},cachedConfig:{labelCls:null,requiredCls:Ext.baseCSSPrefix+'field-required',inputCls:null},getElementConfig:function(){var prefix=Ext.baseCSSPrefix;return{reference:'element',className:'x-container',children:[{reference:'label',cls:prefix+'form-label',children:[{reference:'labelspan',tag:'span'}]},{reference:'innerElement',cls:prefix+'component-outer'}]};},updateLabel:function(newLabel,oldLabel){var renderElement=this.renderElement,prefix=Ext.baseCSSPrefix;if(newLabel){this.labelspan.setHtml(newLabel);renderElement.addCls(prefix+'field-labeled');}else{renderElement.removeCls(prefix+'field-labeled');}},updateLabelAlign:function(newLabelAlign,oldLabelAlign){var renderElement=this.renderElement,prefix=Ext.baseCSSPrefix;if(newLabelAlign){renderElement.addCls(prefix+'label-align-'+newLabelAlign);if(newLabelAlign=="top"){this.label.setWidth('100%');}else{this.updateLabelWidth(this.getLabelWidth());}}
if(oldLabelAlign){renderElement.removeCls(prefix+'label-align-'+oldLabelAlign);}},updateLabelCls:function(newLabelCls,oldLabelCls){if(newLabelCls){this.label.addCls(newLabelCls);}
if(oldLabelCls){this.label.removeCls(oldLabelCls);}},updateLabelWidth:function(newLabelWidth){if(newLabelWidth){if(this.getLabelAlign()=="top"){this.label.setWidth('100%');}else{this.label.setWidth(newLabelWidth);}}},updateRequired:function(newRequired){this.renderElement[newRequired?'addCls':'removeCls'](this.getRequiredCls());},updateRequiredCls:function(newRequiredCls,oldRequiredCls){if(this.getRequired()){this.renderElement.replaceCls(oldRequiredCls,newRequiredCls);}},initialize:function(){var me=this;me.callParent();me.doInitValue();},doInitValue:function(){this.originalValue=this.getInitialConfig().value;},reset:function(){this.setValue(this.originalValue);return this;},isDirty:function(){return false;}},function(){var prototype=this.prototype;this.override({constructor:function(config){config=config||{};var deprecateProperty=function(property,obj,newProperty){if(config.hasOwnProperty(property)){if(obj){config[obj]=config[obj]||{};config[obj][(newProperty)?newProperty:property]=config[obj][(newProperty)?newProperty:property]||config[property];}else{config[newProperty]=config[property];}
delete config[property];Ext.Logger.deprecate("'"+property+"' config is deprecated, use the '"+((obj)?obj+".":"")+((newProperty)?newProperty:property)+"' config instead",2);}};deprecateProperty('fieldCls',null,'inputCls');deprecateProperty('fieldLabel',null,'label');deprecateProperty('useClearIcon',null,'clearIcon');if(config.hasOwnProperty('autoCreateField')){Ext.Logger.deprecate("'autoCreateField' config is deprecated. If you are subclassing Ext.field.Field and you do not want a Ext.field.Input, set the 'input' config to false.",this);}
this.callOverridden(arguments);}});Ext.Object.defineProperty(prototype,'fieldEl',{get:function(){Ext.Logger.deprecate("'fieldEl' is deprecated, please use getInput() to get an instance of Ext.field.Field instead",this);return this.getInput().input;}});Ext.Object.defineProperty(prototype,'labelEl',{get:function(){Ext.Logger.deprecate("'labelEl' is deprecated",this);return this.getLabel().element;}});});Ext.define('Ext.field.Text',{extend:'Ext.field.Field',xtype:'textfield',alternateClassName:'Ext.form.Text',config:{ui:'text',clearIcon:true,placeHolder:null,maxLength:null,autoComplete:null,autoCapitalize:null,autoCorrect:null,readOnly:null,component:{xtype:'input',type:'text'}},initialize:function(){var me=this;me.callParent();me.getComponent().on({scope:this,keyup:'onKeyUp',change:'onChange',focus:'onFocus',blur:'onBlur',paste:'onPaste',mousedown:'onMouseDown',clearicontap:'onClearIconTap'});me.originalValue=me.originalValue||"";me.getComponent().originalValue=me.originalValue;me.syncEmptyCls();},syncEmptyCls:function(){var empty=(this._value)?this._value.length:false,cls=Ext.baseCSSPrefix+'empty';if(empty){this.removeCls(cls);}else{this.addCls(cls);}},updateValue:function(newValue){var component=this.getComponent();if(component){component.setValue(newValue);}
this[newValue?'showClearIcon':'hideClearIcon']();this.syncEmptyCls();},getValue:function(){var me=this;me._value=me.getComponent().getValue();me.syncEmptyCls();return me._value;},updatePlaceHolder:function(newPlaceHolder){this.getComponent().setPlaceHolder(newPlaceHolder);},updateMaxLength:function(newMaxLength){this.getComponent().setMaxLength(newMaxLength);},updateAutoComplete:function(newAutoComplete){this.getComponent().setAutoComplete(newAutoComplete);},updateAutoCapitalize:function(newAutoCapitalize){this.getComponent().setAutoCapitalize(newAutoCapitalize);},updateAutoCorrect:function(newAutoCorrect){this.getComponent().setAutoCorrect(newAutoCorrect);},updateReadOnly:function(newReadOnly){if(newReadOnly){this.hideClearIcon();}else{this.showClearIcon();}
this.getComponent().setReadOnly(newReadOnly);},updateInputType:function(newInputType){var component=this.getComponent();if(component){component.setType(newInputType);}},updateName:function(newName){var component=this.getComponent();if(component){component.setName(newName);}},updateTabIndex:function(newTabIndex){var component=this.getComponent();if(component){component.setTabIndex(newTabIndex);}},updateInputCls:function(newInputCls,oldInputCls){var component=this.getComponent();if(component){component.replaceCls(oldInputCls,newInputCls);}},doSetDisabled:function(disabled){var me=this;me.callParent(arguments);var component=me.getComponent();if(component){component.setDisabled(disabled);}
if(disabled){me.hideClearIcon();}else{me.showClearIcon();}},showClearIcon:function(){var me=this;if(!me.getDisabled()&&!me.getReadOnly()&&me.getValue()&&me.getClearIcon()){me.element.addCls(Ext.baseCSSPrefix+'field-clearable');}
return me;},hideClearIcon:function(){if(this.getClearIcon()){this.element.removeCls(Ext.baseCSSPrefix+'field-clearable');}},onKeyUp:function(e){this.fireAction('keyup',[this,e],'doKeyUp');},doKeyUp:function(me,e){var value=me.getValue();me[value?'showClearIcon':'hideClearIcon']();if(e.browserEvent.keyCode===13){me.fireAction('action',[me,e],'doAction');}},doAction:function(){this.blur();},onClearIconTap:function(e){this.fireAction('clearicontap',[this,e],'doClearIconTap');},doClearIconTap:function(me,e){me.setValue('');me.getValue();},onChange:function(me,value,startValue){me.fireEvent('change',this,value,startValue);},onFocus:function(e){this.isFocused=true;this.fireEvent('focus',this,e);},onBlur:function(e){var me=this;this.isFocused=false;me.fireEvent('blur',me,e);setTimeout(function(){me.isFocused=false;},50);},onPaste:function(e){this.fireEvent('paste',this,e);},onMouseDown:function(e){this.fireEvent('mousedown',this,e);},focus:function(){this.getComponent().focus();return this;},blur:function(){this.getComponent().blur();return this;},select:function(){this.getComponent().select();return this;},reset:function(){this.getComponent().reset();this.getValue();this[this._value?'showClearIcon':'hideClearIcon']();},isDirty:function(){var component=this.getComponent();if(component){return component.isDirty();}
return false;}});Ext.define('Ext.field.TextAreaInput',{extend:'Ext.field.Input',xtype:'textareainput',tag:'textarea'});Ext.define('Ext.field.TextArea',{extend:'Ext.field.Text',xtype:'textareafield',requires:['Ext.field.TextAreaInput'],alternateClassName:'Ext.form.TextArea',config:{ui:'textarea',autoCapitalize:false,component:{xtype:'textareainput'},maxRows:null},updateMaxRows:function(newRows){this.getComponent().setMaxRows(newRows);},doSetHeight:function(newHeight){this.callParent(arguments);var component=this.getComponent();component.input.setHeight(newHeight);},doSetWidth:function(newWidth){this.callParent(arguments);var component=this.getComponent();component.input.setWidth(newWidth);},doKeyUp:function(me){var value=me.getValue();me[value?'showClearIcon':'hideClearIcon']();}});Ext.define('Ext.scroll.Scroller',{extend:'Ext.Evented',requires:['Ext.fx.easing.BoundMomentum','Ext.fx.easing.EaseOut','Ext.util.SizeMonitor','Ext.util.Translatable'],config:{element:null,direction:'auto',translationMethod:'auto',fps:'auto',disabled:null,directionLock:false,momentumEasing:{momentum:{acceleration:30,friction:0.5},bounce:{acceleration:30,springTension:0.3},minVelocity:1},bounceEasing:{duration:400},outOfBoundRestrictFactor:0.5,startMomentumResetTime:300,maxAbsoluteVelocity:6,containerSize:'auto',containerScrollSize:'auto',size:'auto',autoRefresh:true,initialOffset:{x:0,y:0},slotSnapSize:{x:0,y:0},slotSnapOffset:{x:0,y:0},slotSnapEasing:{duration:150}},cls:Ext.baseCSSPrefix+'scroll-scroller',containerCls:Ext.baseCSSPrefix+'scroll-container',dragStartTime:0,dragEndTime:0,isDragging:false,isAnimating:false,constructor:function(config){var element=config&&config.element;this.doAnimationFrame=Ext.Function.bind(this.doAnimationFrame,this);this.stopAnimation=Ext.Function.bind(this.stopAnimation,this);this.listeners={scope:this,touchstart:'onTouchStart',touchend:'onTouchEnd',dragstart:'onDragStart',drag:'onDrag',dragend:'onDragEnd'};this.minPosition={x:0,y:0};this.startPosition={x:0,y:0};this.size={x:0,y:0};this.position={x:0,y:0};this.velocity={x:0,y:0};this.isAxisEnabledFlags={x:false,y:false};this.flickStartPosition={x:0,y:0};this.flickStartTime={x:0,y:0};this.lastDragPosition={x:0,y:0};this.dragDirection={x:0,y:0};this.initialConfig=config;if(element){this.setElement(element);}
return this;},applyElement:function(element){if(!element){return;}
return Ext.get(element);},updateElement:function(element){this.initialize();element.addCls(this.cls);if(!this.getDisabled()){this.attachListeneners();}
this.onConfigUpdate(['containerSize','size'],'refreshMaxPosition');this.on('maxpositionchange','snapToBoundary');this.on('minpositionchange','snapToBoundary');return this;},getTranslatable:function(){if(!this.hasOwnProperty('translatable')){var bounceEasing=this.getBounceEasing();this.translatable=new Ext.util.Translatable({translationMethod:this.getTranslationMethod(),element:this.getElement(),easingX:bounceEasing.x,easingY:bounceEasing.y,useWrapper:false,listeners:{animationframe:'onAnimationFrame',animationend:'onAnimationEnd',axisanimationend:'onAxisAnimationEnd',scope:this}});}
return this.translatable;},updateFps:function(fps){if(fps!=='auto'){this.getTranslatable().setFps(fps);}},attachListeneners:function(){this.getContainer().on(this.listeners);},detachListeners:function(){this.getContainer().un(this.listeners);},updateDisabled:function(disabled){if(disabled){this.detachListeners();}
else{this.attachListeneners();}},updateInitialOffset:function(initialOffset){if(typeof initialOffset=='number'){initialOffset={x:initialOffset,y:initialOffset};}
var position=this.position,x,y;position.x=x=initialOffset.x;position.y=y=initialOffset.y;this.getTranslatable().doTranslate(-x,-y);},applyDirection:function(direction){var minPosition=this.getMinPosition(),maxPosition=this.getMaxPosition(),isHorizontal,isVertical;this.givenDirection=direction;if(direction==='auto'){isHorizontal=maxPosition.x>minPosition.x;isVertical=maxPosition.y>minPosition.y;if(isHorizontal&&isVertical){direction='both';}
else if(isHorizontal){direction='horizontal';}
else{direction='vertical';}}
return direction;},updateDirection:function(direction){var isAxisEnabled=this.isAxisEnabledFlags;isAxisEnabled.x=(direction==='both'||direction==='horizontal');isAxisEnabled.y=(direction==='both'||direction==='vertical');},isAxisEnabled:function(axis){this.getDirection();return this.isAxisEnabledFlags[axis];},applyMomentumEasing:function(easing){var defaultClass=Ext.fx.easing.BoundMomentum;return{x:Ext.factory(easing,defaultClass),y:Ext.factory(easing,defaultClass)};},applyBounceEasing:function(easing){var defaultClass=Ext.fx.easing.EaseOut;return{x:Ext.factory(easing,defaultClass),y:Ext.factory(easing,defaultClass)};},applySlotSnapEasing:function(easing){var defaultClass=Ext.fx.easing.EaseOut;return{x:Ext.factory(easing,defaultClass),y:Ext.factory(easing,defaultClass)};},getMinPosition:function(){var minPosition=this.minPosition;if(!minPosition){this.minPosition=minPosition={x:0,y:0};this.fireEvent('minpositionchange',this,minPosition);}
return minPosition;},getMaxPosition:function(){var maxPosition=this.maxPosition,size,containerSize;if(!maxPosition){size=this.getSize();containerSize=this.getContainerSize();this.maxPosition=maxPosition={x:Math.max(0,size.x-containerSize.x),y:Math.max(0,size.y-containerSize.y)};this.fireEvent('maxpositionchange',this,maxPosition);}
return maxPosition;},refreshMaxPosition:function(){this.maxPosition=null;this.getMaxPosition();},applyContainerSize:function(size){var containerDom=this.getContainer().dom,x,y;if(!containerDom){return;}
this.givenContainerSize=size;if(size==='auto'){x=containerDom.offsetWidth;y=containerDom.offsetHeight;}
else{x=size.x;y=size.y;}
return{x:x,y:y};},applySize:function(size){var dom=this.getElement().dom,x,y;if(!dom){return;}
this.givenSize=size;if(size==='auto'){x=dom.offsetWidth;y=dom.offsetHeight;}
else{x=size.x;y=size.y;}
return{x:x,y:y};},applyContainerScrollSize:function(size){var containerDom=this.getContainer().dom,x,y;if(!containerDom){return;}
this.givenContainerScrollSize=size;if(size==='auto'){x=containerDom.scrollWidth;y=containerDom.scrollHeight;}
else{x=size.x;y=size.y;}
return{x:x,y:y};},updateAutoRefresh:function(autoRefresh){var SizeMonitor=Ext.util.SizeMonitor,sizeMonitors;if(autoRefresh){this.sizeMonitors={element:new SizeMonitor({element:this.getElement(),callback:this.doRefresh,scope:this}),container:new SizeMonitor({element:this.getContainer(),callback:this.doRefresh,scope:this})};}
else{sizeMonitors=this.sizeMonitors;if(sizeMonitors){sizeMonitors.element.destroy();sizeMonitors.container.destroy();}}},applySlotSnapSize:function(snapSize){if(typeof snapSize=='number'){return{x:snapSize,y:snapSize}}
return snapSize;},applySlotSnapOffset:function(snapOffset){if(typeof snapOffset=='number'){return{x:snapOffset,y:snapOffset}}
return snapOffset;},getContainer:function(){var container=this.container;if(!container){this.container=container=this.getElement().getParent();if(!container){Ext.Logger.error("Making an element scrollable that doesn't have any container");}
container.addCls(this.containerCls);}
return container;},doRefresh:function(){this.stopAnimation();this.getTranslatable().refresh();this.setSize(this.givenSize);this.setContainerSize(this.givenContainerSize);this.setContainerScrollSize(this.givenContainerScrollSize);this.setDirection(this.givenDirection);this.fireEvent('refresh',this);},refresh:function(){var sizeMonitors=this.sizeMonitors;if(sizeMonitors){sizeMonitors.element.refresh();sizeMonitors.container.refresh();}
this.doRefresh();return this;},scrollTo:function(x,y,animation){if(typeof x!='number'&&arguments.length===1){Ext.Logger.deprecate("Calling scrollTo() with an object argument is deprecated, "+"please pass x and y arguments instead",this);y=x.y;x=x.x;}
var translatable=this.getTranslatable(),position=this.position,positionChanged=false,translationX,translationY;if(this.isAxisEnabled('x')){if(typeof x!='number'){x=position.x;}
else{if(position.x!==x){position.x=x;positionChanged=true;}}
translationX=-x;}
if(this.isAxisEnabled('y')){if(typeof y!='number'){y=position.y;}
else{if(position.y!==y){position.y=y;positionChanged=true;}}
translationY=-y;}
if(positionChanged){if(animation!==undefined){translatable.translateAnimated(translationX,translationY,animation);}
else{this.fireEvent('scroll',this,position.x,position.y);translatable.doTranslate(translationX,translationY);}}
return this;},scrollToTop:function(animation){var initialOffset=this.getInitialOffset();return this.scrollTo(initialOffset.x,initialOffset.y,animation);},scrollToEnd:function(animation){return this.scrollTo(0,this.getSize().y-this.getContainerSize().y,animation);},scrollBy:function(x,y,animation){var position=this.position;x=(typeof x=='number')?x+position.x:null;y=(typeof y=='number')?y+position.y:null;return this.scrollTo(x,y,animation);},onTouchStart:function(){this.isTouching=true;this.stopAnimation();},onTouchEnd:function(){var position=this.position;this.isTouching=false;if(!this.isDragging&&this.snapToSlot()){this.fireEvent('scrollstart',this,position.x,position.y);}},onDragStart:function(e){var direction=this.getDirection(),absDeltaX=e.absDeltaX,absDeltaY=e.absDeltaY,directionLock=this.getDirectionLock(),startPosition=this.startPosition,flickStartPosition=this.flickStartPosition,flickStartTime=this.flickStartTime,lastDragPosition=this.lastDragPosition,currentPosition=this.position,dragDirection=this.dragDirection,x=currentPosition.x,y=currentPosition.y,now=Ext.Date.now();this.isDragging=true;if(directionLock&&direction!=='both'){if((direction==='horizontal'&&absDeltaX>absDeltaY)||(direction==='vertical'&&absDeltaY>absDeltaX)){e.stopPropagation();}
else{this.isDragging=false;return;}}
lastDragPosition.x=x;lastDragPosition.y=y;flickStartPosition.x=x;flickStartPosition.y=y;startPosition.x=x;startPosition.y=y;flickStartTime.x=now;flickStartTime.y=now;dragDirection.x=0;dragDirection.y=0;this.dragStartTime=now;this.isDragging=true;this.fireEvent('scrollstart',this,x,y);},onAxisDrag:function(axis,delta){if(!this.isAxisEnabled(axis)){return;}
var flickStartPosition=this.flickStartPosition,flickStartTime=this.flickStartTime,lastDragPosition=this.lastDragPosition,dragDirection=this.dragDirection,old=this.position[axis],min=this.getMinPosition()[axis],max=this.getMaxPosition()[axis],start=this.startPosition[axis],last=lastDragPosition[axis],current=start-delta,lastDirection=dragDirection[axis],restrictFactor=this.getOutOfBoundRestrictFactor(),startMomentumResetTime=this.getStartMomentumResetTime(),now=Ext.Date.now(),distance;if(current<min){current*=restrictFactor;}
else if(current>max){distance=current-max;current=max+distance*restrictFactor;}
if(current>last){dragDirection[axis]=1;}
else if(current<last){dragDirection[axis]=-1;}
if((lastDirection!==0&&(dragDirection[axis]!==lastDirection))||(now-flickStartTime[axis])>startMomentumResetTime){flickStartPosition[axis]=old;flickStartTime[axis]=now;}
lastDragPosition[axis]=current;},onDrag:function(e){if(!this.isDragging){return;}
var lastDragPosition=this.lastDragPosition;this.onAxisDrag('x',e.deltaX);this.onAxisDrag('y',e.deltaY);this.scrollTo(lastDragPosition.x,lastDragPosition.y);},onDragEnd:function(e){var easingX,easingY;if(!this.isDragging){return;}
this.dragEndTime=Ext.Date.now();this.onDrag(e);this.isDragging=false;easingX=this.getAnimationEasing('x');easingY=this.getAnimationEasing('y');if(easingX||easingY){this.getTranslatable().animate(easingX,easingY);}
else{this.onScrollEnd();}},getAnimationEasing:function(axis){if(!this.isAxisEnabled(axis)){return null;}
var currentPosition=this.position[axis],flickStartPosition=this.flickStartPosition[axis],flickStartTime=this.flickStartTime[axis],minPosition=this.getMinPosition()[axis],maxPosition=this.getMaxPosition()[axis],maxAbsVelocity=this.getMaxAbsoluteVelocity(),boundValue=null,dragEndTime=this.dragEndTime,easing,velocity,duration;if(currentPosition<minPosition){boundValue=minPosition;}
else if(currentPosition>maxPosition){boundValue=maxPosition;}
if(boundValue!==null){easing=this.getBounceEasing()[axis];easing.setConfig({startTime:dragEndTime,startValue:-currentPosition,endValue:-boundValue});return easing;}
duration=dragEndTime-flickStartTime;if(duration===0){return null;}
velocity=(currentPosition-flickStartPosition)/(dragEndTime-flickStartTime);if(velocity===0){return null;}
if(velocity<-maxAbsVelocity){velocity=-maxAbsVelocity;}
else if(velocity>maxAbsVelocity){velocity=maxAbsVelocity;}
easing=this.getMomentumEasing()[axis];easing.setConfig({startTime:dragEndTime,startValue:-currentPosition,startVelocity:-velocity,minMomentumValue:-maxPosition,maxMomentumValue:0});return easing;},onAnimationFrame:function(translatable,x,y){var position=this.position;position.x=-x;position.y=-y;this.fireEvent('scroll',this,position.x,position.y);},onAxisAnimationEnd:function(axis){},onAnimationEnd:function(){this.snapToBoundary();this.onScrollEnd();},stopAnimation:function(){this.getTranslatable().stopAnimation();},onScrollEnd:function(){var position=this.position;if(this.isTouching||!this.snapToSlot()){this.fireEvent('scrollend',this,position.x,position.y);}},snapToSlot:function(){var snapX=this.getSnapPosition('x'),snapY=this.getSnapPosition('y'),easing=this.getSlotSnapEasing();if(snapX!==null||snapY!==null){this.scrollTo(snapX,snapY,{easingX:easing.x,easingY:easing.y});return true;}
return false;},getSnapPosition:function(axis){var snapSize=this.getSlotSnapSize()[axis],snapPosition=null,position,snapOffset,maxPosition,mod;if(snapSize!==0&&this.isAxisEnabled(axis)){position=this.position[axis];snapOffset=this.getSlotSnapOffset()[axis];maxPosition=this.getMaxPosition()[axis];mod=(position-snapOffset)%snapSize;if(mod!==0){if(Math.abs(mod)>snapSize/2){snapPosition=position+((mod>0)?snapSize-mod:mod-snapSize);if(snapPosition>maxPosition){snapPosition=position-mod;}}
else{snapPosition=position-mod;}}}
return snapPosition;},snapToBoundary:function(){var position=this.position,minPosition=this.getMinPosition(),maxPosition=this.getMaxPosition(),minX=minPosition.x,minY=minPosition.y,maxX=maxPosition.x,maxY=maxPosition.y,x=Math.round(position.x),y=Math.round(position.y);if(x<minX){x=minX;}
else if(x>maxX){x=maxX;}
if(y<minY){y=minY;}
else if(y>maxY){y=maxY;}
this.scrollTo(x,y);},destroy:function(){var element=this.getElement(),sizeMonitors=this.sizeMonitors;if(sizeMonitors){sizeMonitors.element.destroy();sizeMonitors.container.destroy();}
if(element&&!element.isDestroyed){element.removeCls(this.cls);this.getContainer().removeCls(this.containerCls);}
Ext.destroy(this.translatable);this.callParent(arguments);}},function(){this.override({constructor:function(config){var element,acceleration,slotSnapOffset,friction,springTension,minVelocity;if(!config){config={};}
if(typeof config=='string'){config={direction:config};}
if(arguments.length==2){Ext.Logger.deprecate("Passing element as the first argument is deprecated, pass it as the "+"'element' property of the config object instead");element=config;config=arguments[1];if(!config){config={};}
config.element=element;}
if(config.hasOwnProperty('acceleration')){acceleration=config.acceleration;delete config.acceleration;Ext.Logger.deprecate("'acceleration' config is deprecated, set momentumEasing.momentum.acceleration and momentumEasing.bounce.acceleration configs instead");Ext.merge(config,{momentumEasing:{momentum:{acceleration:acceleration},bounce:{acceleration:acceleration}}});}
if(config.hasOwnProperty('snap')){config.slotSnapOffset=config.snap;Ext.Logger.deprecate("'snap' config is deprecated, please use the 'slotSnapOffset' config instead");}
if(config.hasOwnProperty('friction')){friction=config.friction;delete config.friction;Ext.Logger.deprecate("'friction' config is deprecated, set momentumEasing.momentum.friction config instead");Ext.merge(config,{momentumEasing:{momentum:{friction:friction}}});}
if(config.hasOwnProperty('springTension')){springTension=config.springTension;delete config.springTension;Ext.Logger.deprecate("'springTension' config is deprecated, set momentumEasing.momentum.springTension config instead");Ext.merge(config,{momentumEasing:{momentum:{springTension:springTension}}});}
if(config.hasOwnProperty('minVelocityForAnimation')){minVelocity=config.minVelocityForAnimation;delete config.minVelocityForAnimation;Ext.Logger.deprecate("'minVelocityForAnimation' config is deprecated, set momentumEasing.minVelocity config instead");Ext.merge(config,{momentumEasing:{minVelocity:minVelocity}});}
this.callOverridden(arguments);},scrollToAnimated:function(x,y,animation){Ext.Logger.deprecate("scrollToAnimated() is deprecated, please use scrollTo() and pass 'animation' as "+"the third argument instead");return this.scrollTo.apply(this,arguments);},scrollBy:function(x,y,animation){if(Ext.isObject(x)){Ext.Logger.deprecate("calling scrollBy() with an object of x and y properties is no longer supported. "+"Please pass x and y values as two separate arguments instead");y=x.y;x=x.x;}
return this.callOverridden([x,y,animation]);},setOffset:function(offset){return this.scrollToAnimated(-offset.x,-offset.y);}});});Ext.define('Ext.scroll.indicator.Abstract',{extend:'Ext.Component',config:{baseCls:'x-scroll-indicator',axis:'x',value:0,length:null,hidden:true,ui:'dark'},cachedConfig:{ratio:1,barCls:'x-scroll-bar',active:true},barElement:null,barLength:0,gapLength:0,getElementConfig:function(){return{reference:'barElement',children:[this.callParent()]};},applyRatio:function(ratio){if(isNaN(ratio)){ratio=1;}
return ratio;},refresh:function(){var bar=this.barElement,barDom=bar.dom,ratio=this.getRatio(),axis=this.getAxis(),barLength=(axis==='x')?barDom.offsetWidth:barDom.offsetHeight,length=barLength*ratio;this.barLength=barLength;this.gapLength=barLength-length;this.setLength(length);this.updateValue(this.getValue());},updateBarCls:function(barCls){this.barElement.addCls(barCls);},updateAxis:function(axis){this.element.addCls(this.getBaseCls(),null,axis);this.barElement.addCls(this.getBarCls(),null,axis);},updateValue:function(value){this.setOffset(this.gapLength*value);},updateActive:function(active){this.barElement[active?'addCls':'removeCls']('active');},doSetHidden:function(hidden){var elementDomStyle=this.element.dom.style;if(hidden){elementDomStyle.opacity='0';}
else{elementDomStyle.opacity='';}},updateLength:function(length){var axis=this.getAxis(),element=this.element;if(axis==='x'){element.setWidth(length);}
else{element.setHeight(length);}},setOffset:function(offset){var axis=this.getAxis(),element=this.element;if(axis==='x'){element.setLeft(offset);}
else{element.setTop(offset);}}});Ext.define('Ext.scroll.indicator.Default',{extend:'Ext.scroll.indicator.Abstract',config:{cls:'default'},setOffset:function(offset){var axis=this.getAxis(),domStyle=this.element.dom.style;if(axis==='x'){domStyle.webkitTransform='translate3d('+offset+'px, 0, 0)';}
else{domStyle.webkitTransform='translate3d(0, '+offset+'px, 0)';}},applyLength:function(length){return Math.round(Math.max(0,length));},updateValue:function(value){var barLength=this.barLength,gapLength=this.gapLength,length=this.getLength(),newLength,offset,extra;if(value<=0){offset=0;this.updateLength(this.applyLength(length+value*barLength));}
else if(value>=1){extra=Math.round((value-1)*barLength);newLength=this.applyLength(length-extra);extra=length-newLength;this.updateLength(newLength);offset=gapLength+extra;}
else{offset=gapLength*value;}
this.setOffset(offset);}});Ext.define('Ext.scroll.indicator.ScrollPosition',{extend:'Ext.scroll.indicator.Abstract',config:{cls:'scrollposition'},getElementConfig:function(){var config=this.callParent(arguments);config.children.unshift({className:'x-scroll-bar-stretcher'});return config;},updateValue:function(value){if(this.gapLength===0){if(value>1){value=value-1;}
this.setOffset(this.barLength*value);}
else{this.setOffset(this.gapLength*value);}},setLength:function(length){var axis=this.getAxis(),scrollOffset=this.barLength,barDom=this.barElement.dom,element=this.element;this.callParent(arguments);if(axis==='x'){barDom.scrollLeft=scrollOffset;element.setLeft(scrollOffset);}
else{barDom.scrollTop=scrollOffset;element.setTop(scrollOffset);}},setOffset:function(offset){var axis=this.getAxis(),scrollOffset=this.barLength,barDom=this.barElement.dom;offset=scrollOffset-offset;if(axis==='x'){barDom.scrollLeft=offset;}
else{barDom.scrollTop=offset;}}});Ext.define('Ext.scroll.indicator.CssTransform',{extend:'Ext.scroll.indicator.Abstract',config:{cls:'csstransform'},getElementConfig:function(){var config=this.callParent();config.children[0].children=[{reference:'startElement'},{reference:'middleElement'},{reference:'endElement'}];return config;},refresh:function(){var axis=this.getAxis(),startElementDom=this.startElement.dom,endElementDom=this.endElement.dom,middleElement=this.middleElement,startElementLength,endElementLength;if(axis==='x'){startElementLength=startElementDom.offsetWidth;endElementLength=endElementDom.offsetWidth;middleElement.setLeft(startElementLength);}
else{startElementLength=startElementDom.offsetHeight;endElementLength=endElementDom.offsetHeight;middleElement.setTop(startElementLength);}
this.startElementLength=startElementLength;this.endElementLength=endElementLength;this.minLength=startElementLength+endElementLength;this.callParent();},applyLength:function(length){return Math.round(Math.max(this.minLength,length));},updateLength:function(length){var axis=this.getAxis(),endElementStyle=this.endElement.dom.style,middleElementStyle=this.middleElement.dom.style,endElementLength=this.endElementLength,endElementOffset=length-endElementLength,middleElementLength=endElementOffset-this.startElementLength;if(axis==='x'){endElementStyle.webkitTransform='translate3d('+endElementOffset+'px, 0, 0)';middleElementStyle.webkitTransform='translate3d(0, 0, 0) scaleX('+middleElementLength+')';}
else{endElementStyle.webkitTransform='translate3d(0, '+endElementOffset+'px, 0)';middleElementStyle.webkitTransform='translate3d(0, 0, 0) scaleY('+middleElementLength+')';}},updateValue:function(value){var barLength=this.barLength,gapLength=this.gapLength,length=this.getLength(),newLength,offset,extra;if(value<=0){offset=0;this.updateLength(this.applyLength(length+value*barLength));}
else if(value>=1){extra=Math.round((value-1)*barLength);newLength=this.applyLength(length-extra);extra=length-newLength;this.updateLength(newLength);offset=gapLength+extra;}
else{offset=gapLength*value;}
this.setOffset(offset);},setOffset:function(offset){var axis=this.getAxis(),elementStyle=this.element.dom.style;offset=Math.round(offset);if(axis==='x'){elementStyle.webkitTransform='translate3d('+offset+'px, 0, 0)';}
else{elementStyle.webkitTransform='translate3d(0, '+offset+'px, 0)';}}});Ext.define('Ext.scroll.Indicator',{requires:['Ext.scroll.indicator.Default','Ext.scroll.indicator.ScrollPosition','Ext.scroll.indicator.CssTransform'],alternateClassName:'Ext.util.Indicator',constructor:function(config){if(Ext.os.is.Android2||Ext.browser.is.ChromeMobile){return new Ext.scroll.indicator.ScrollPosition(config);}
else if(Ext.os.is.iOS){return new Ext.scroll.indicator.CssTransform(config);}
else{return new Ext.scroll.indicator.Default(config);}}});Ext.define('Ext.scroll.View',{extend:'Ext.Evented',alternateClassName:'Ext.util.ScrollView',requires:['Ext.scroll.Scroller','Ext.scroll.Indicator'],config:{indicatorsUi:'dark',element:null,scroller:{},indicators:{x:{axis:'x'},y:{axis:'y'}},indicatorsHidingDelay:100,cls:Ext.baseCSSPrefix+'scroll-view'},processConfig:function(config){if(!config){return null;}
if(typeof config=='string'){config={direction:config};}
config=Ext.merge({},config);var scrollerConfig=config.scroller,name;if(!scrollerConfig){config.scroller=scrollerConfig={};}
for(name in config){if(config.hasOwnProperty(name)){if(!this.hasConfig(name)){scrollerConfig[name]=config[name];delete config[name];}}}
return config;},constructor:function(config){config=this.processConfig(config);this.useIndicators={x:true,y:true};this.doHideIndicators=Ext.Function.bind(this.doHideIndicators,this);this.initConfig(config);},setConfig:function(config){return this.callParent([this.processConfig(config)]);},updateIndicatorsUi:function(newUi){var indicators=this.getIndicators();indicators.x.setUi(newUi);indicators.y.setUi(newUi);},applyScroller:function(config,currentScroller){return Ext.factory(config,Ext.scroll.Scroller,currentScroller);},applyIndicators:function(config,indicators){var defaultClass=Ext.scroll.Indicator,useIndicators=this.useIndicators;if(!config){config={};}
if(!config.x){useIndicators.x=false;config.x={};}
if(!config.y){useIndicators.y=false;config.y={};}
return{x:Ext.factory(config.x,defaultClass,indicators&&indicators.x),y:Ext.factory(config.y,defaultClass,indicators&&indicators.y)};},updateIndicators:function(indicators){this.indicatorsGrid=Ext.Element.create({className:'x-scroll-bar-grid-wrapper',children:[{className:'x-scroll-bar-grid',children:[{children:[{},{children:[indicators.y.barElement]}]},{children:[{children:[indicators.x.barElement]},{}]}]}]});},updateScroller:function(scroller){scroller.on({scope:this,scrollstart:'onScrollStart',scroll:'onScroll',scrollend:'onScrollEnd',refresh:'refreshIndicators'});},isAxisEnabled:function(axis){return this.getScroller().isAxisEnabled(axis)&&this.useIndicators[axis];},applyElement:function(element){if(element){return Ext.get(element);}},updateElement:function(element){var scrollerElement=element.getFirstChild().getFirstChild(),scroller=this.getScroller();element.addCls(this.getCls());element.insertFirst(this.indicatorsGrid);scroller.setElement(scrollerElement);this.refreshIndicators();return this;},showIndicators:function(){var indicators=this.getIndicators();if(this.hasOwnProperty('indicatorsHidingTimer')){clearTimeout(this.indicatorsHidingTimer);delete this.indicatorsHidingTimer;}
if(this.isAxisEnabled('x')){indicators.x.show();}
if(this.isAxisEnabled('y')){indicators.y.show();}},hideIndicators:function(){var delay=this.getIndicatorsHidingDelay();if(delay>0){this.indicatorsHidingTimer=setTimeout(this.doHideIndicators,delay);}
else{this.doHideIndicators();}},doHideIndicators:function(){var indicators=this.getIndicators();if(this.isAxisEnabled('x')){indicators.x.hide();}
if(this.isAxisEnabled('y')){indicators.y.hide();}},onScrollStart:function(){this.onScroll.apply(this,arguments);this.showIndicators();},onScrollEnd:function(){this.hideIndicators();},onScroll:function(scroller,x,y){this.setIndicatorValue('x',x);this.setIndicatorValue('y',y);if(this.isBenchmarking){this.framesCount++;}},isBenchmarking:false,framesCount:0,getCurrentFps:function(){var now=Date.now(),fps;if(!this.isBenchmarking){this.isBenchmarking=true;fps=0;}
else{fps=Math.round(this.framesCount*1000/(now-this.framesCountStartTime));}
this.framesCountStartTime=now;this.framesCount=0;return fps;},setIndicatorValue:function(axis,scrollerPosition){if(!this.isAxisEnabled(axis)){return this;}
var scroller=this.getScroller(),scrollerMaxPosition=scroller.getMaxPosition()[axis],scrollerContainerSize=scroller.getContainerSize()[axis],value;if(scrollerMaxPosition===0){value=scrollerPosition/scrollerContainerSize;if(scrollerPosition>=0){value+=1;}}
else{if(scrollerPosition>scrollerMaxPosition){value=1+((scrollerPosition-scrollerMaxPosition)/scrollerContainerSize);}
else if(scrollerPosition<0){value=scrollerPosition/scrollerContainerSize;}
else{value=scrollerPosition/scrollerMaxPosition;}}
this.getIndicators()[axis].setValue(value);},refreshIndicator:function(axis){if(!this.isAxisEnabled(axis)){return this;}
var scroller=this.getScroller(),indicator=this.getIndicators()[axis],scrollerContainerSize=scroller.getContainerSize()[axis],scrollerSize=scroller.getSize()[axis],ratio=scrollerContainerSize/scrollerSize;indicator.setRatio(ratio);indicator.refresh();},refresh:function(){return this.getScroller().refresh();},refreshIndicators:function(){var indicators=this.getIndicators();indicators.x.setActive(this.isAxisEnabled('x'));indicators.y.setActive(this.isAxisEnabled('y'));this.refreshIndicator('x');this.refreshIndicator('y');},destroy:function(){var element=this.getElement(),indicators=this.getIndicators();if(element&&!element.isDestroyed){element.removeCls(this.getCls());}
indicators.x.destroy();indicators.y.destroy();Ext.destroy(this.getScroller(),this.indicatorsGrid);delete this.indicatorsGrid;this.callParent(arguments);}});Ext.define('Ext.behavior.Scrollable',{extend:'Ext.behavior.Behavior',requires:['Ext.scroll.View'],constructor:function(){this.listeners={painted:'onComponentPainted',scope:this};this.callParent(arguments);},onComponentPainted:function(){this.scrollView.refresh();},setConfig:function(config){var scrollView=this.scrollView,component=this.component,scrollViewElement,scrollContainer,scrollerElement;if(config){if(!scrollView){this.scrollView=scrollView=new Ext.scroll.View(config);scrollView.on('destroy','onScrollViewDestroy',this);component.setUseBodyElement(true);this.scrollerElement=scrollerElement=component.innerElement;this.scrollContainer=scrollContainer=scrollerElement.wrap();this.scrollViewElement=scrollViewElement=component.bodyElement;scrollView.setElement(scrollViewElement);if(component.isPainted()){this.onComponentPainted(component);}
component.on(this.listeners);}
else if(Ext.isObject(config)){scrollView.setConfig(config);}}
else if(scrollView){scrollView.destroy();}
return this;},getScrollView:function(){return this.scrollView;},onScrollViewDestroy:function(){var component=this.component,scrollerElement=this.scrollerElement;if(!scrollerElement.isDestroyed){this.scrollerElement.unwrap();}
this.scrollContainer.destroy();component.un(this.listeners);delete this.scrollerElement;delete this.scrollView;delete this.scrollContainer;},onComponentDestroy:function(){var scrollView=this.scrollView;if(scrollView){scrollView.destroy();}}});Ext.define('Ext.Container',{extend:'Ext.Component',alternateClassName:'Ext.lib.Container',requires:['Ext.layout.Layout','Ext.ItemCollection','Ext.behavior.Scrollable','Ext.Mask'],xtype:'container',eventedConfig:{activeItem:0},config:{layout:null,control:{},defaults:null,items:null,autoDestroy:true,defaultType:null,scrollable:null,useBodyElement:null,masked:null,modal:null,hideOnMaskTap:null},isContainer:true,delegateListeners:{delegate:'> component',centeredchange:'onItemCenteredChange',dockedchange:'onItemDockedChange',floatingchange:'onItemFloatingChange'},constructor:function(config){var me=this;me._items=me.items=new Ext.ItemCollection();me.innerItems=[];me.onItemAdd=me.onFirstItemAdd;me.callParent(arguments);},getElementConfig:function(){return{reference:'element',className:'x-container',children:[{reference:'innerElement',className:'x-inner'}]};},applyMasked:function(masked,currentMask){currentMask=Ext.factory(masked,Ext.Mask,currentMask);if(currentMask){this.add(currentMask);}
return currentMask;},mask:function(mask){this.setMasked(mask||true);},unmask:function(){this.setMasked(false);},applyModal:function(modal,currentMask){if(!modal&&!currentMask){return;}
return Ext.factory(modal,Ext.Mask,currentMask);},updateModal:function(newModal,oldModal){var listeners={painted:'refreshModalMask',erased:'destroyModalMask'};if(newModal){this.on(listeners);newModal.on('destroy','onModalDestroy',this);if(this.getTop()===null&&this.getBottom()===null&&this.getRight()===null&&this.getLeft()===null&&!this.getCentered()){Ext.Logger.warn("You have specified a modal config on a container that is neither centered nor has any positioning information.  Setting to top and left to 0 to compensate.");this.setTop(0);this.setLeft(0);}
if(this.isPainted()){this.refreshModalMask();}}
else if(oldModal){oldModal.un('destroy','onModalDestroy',this);this.un(listeners);}},onModalDestroy:function(){this.setModal(null);},refreshModalMask:function(){var mask=this.getModal(),container=this.getParent();if(!this.painted){this.painted=true;if(mask){container.insertBefore(mask,this);mask.setZIndex(this.getZIndex()-1);if(this.getHideOnMaskTap()){mask.on('tap','hide',this,{single:true});}}}},destroyModalMask:function(){var mask=this.getModal(),container=this.getParent();if(this.painted){this.painted=false;if(mask){mask.un('tap','hide',this);container.remove(mask,false);}}},updateZIndex:function(zIndex){var modal=this.getModal();this.callParent(arguments);if(modal){modal.setZIndex(zIndex-1);}},updateBaseCls:function(newBaseCls,oldBaseCls){var me=this,ui=me.getUi();if(newBaseCls){this.element.addCls(newBaseCls);this.innerElement.addCls(newBaseCls,null,'inner');if(ui){this.element.addCls(newBaseCls,null,ui);}}
if(oldBaseCls){this.element.removeCls(oldBaseCls);this.innerElement.removeCls(newBaseCls,null,'inner');if(ui){this.element.removeCls(oldBaseCls,null,ui);}}},updateUseBodyElement:function(useBodyElement){if(useBodyElement){this.bodyElement=this.innerElement.wrap({cls:'x-body'});this.referenceList.push('bodyElement');}},applyItems:function(items,collection){if(items){this.getDefaultType();this.getDefaults();if(this.initialized&&collection.length>0){this.removeAll();}
this.add(items);}},applyControl:function(selectors){var selector,key,listener,listeners;for(selector in selectors){listeners=selectors[selector];for(key in listeners){listener=listeners[key];if(Ext.isObject(listener)){listener.delegate=selector;}}
listeners.delegate=selector;this.addListener(listeners);}
return selectors;},onFirstItemAdd:function(){delete this.onItemAdd;this.setLayout(new Ext.layout.Layout(this,this.getLayout()||'default'));if(this.innerHtmlElement&&!this.getHtml()){this.innerHtmlElement.destroy();delete this.innerHtmlElement;}
this.on(this.delegateListeners);return this.onItemAdd.apply(this,arguments);},updateLayout:function(newLayout,oldLayout){if(oldLayout&&oldLayout.isLayout){Ext.Logger.error('Replacing a layout after one has already been initialized is not currently supported.');}},updateDefaultType:function(defaultType){this.defaultItemClass=Ext.ClassManager.getByAlias('widget.'+defaultType);if(!this.defaultItemClass){Ext.Logger.error("Invalid defaultType of: '"+defaultType+"', must be a valid component xtype");}},applyDefaults:function(defaults){if(defaults){this.factoryItem=this.factoryItemWithDefaults;return defaults;}},factoryItem:function(item){if(!item){Ext.Logger.error("Invalid item given: "+item+", must be either the config object to factory a new item, "+"or an existing component instance");}
return Ext.factory(item,this.defaultItemClass);},factoryItemWithDefaults:function(item){if(!item){Ext.Logger.error("Invalid item given: "+item+", must be either the config object to factory a new item, "+"or an existing component instance");}
var me=this,defaults=me.getDefaults(),instance;if(!defaults){return Ext.factory(item,me.defaultItemClass);}
if(item.isComponent){instance=item;if(defaults&&item.isInnerItem()&&!me.has(instance)){instance.setConfig(defaults,true);}}
else{if(defaults&&!item.ignoreDefaults){if(!(item.hasOwnProperty('left')&&item.hasOwnProperty('right')&&item.hasOwnProperty('top')&&item.hasOwnProperty('bottom')&&item.hasOwnProperty('docked')&&item.hasOwnProperty('centered'))){item=Ext.mergeIf({},item,defaults);}}
instance=Ext.factory(item,me.defaultItemClass);}
return instance;},add:function(newItems){var me=this,i,ln,item,newActiveItem;newItems=Ext.Array.from(newItems);ln=newItems.length;for(i=0;i<ln;i++){item=me.factoryItem(newItems[i]);this.doAdd(item);if(!newActiveItem&&!this.getActiveItem()&&this.innerItems.length>0&&item.isInnerItem()){newActiveItem=item;}}
if(newActiveItem){this.setActiveItem(newActiveItem);}
return item;},doAdd:function(item){var me=this,items=me.getItems(),index;if(!items.has(item)){index=items.length;items.add(item);if(item.isInnerItem()){me.insertInner(item);}
item.setParent(me);me.onItemAdd(item,index);}},remove:function(item,destroy){var me=this,index=me.indexOf(item),innerItems=me.getInnerItems();if(destroy===undefined){destroy=me.getAutoDestroy();}
if(index!==-1){if(!me.removingAll&&innerItems.length>1&&item===me.getActiveItem()){me.on({activeitemchange:'doRemove',scope:me,single:true,order:'after',args:[item,index,destroy]});me.doResetActiveItem(innerItems.indexOf(item));}
else{me.doRemove(item,index,destroy);if(innerItems.length===0){me.setActiveItem(null);}}}
return me;},doResetActiveItem:function(innerIndex){if(innerIndex===0){this.setActiveItem(1);}
else{this.setActiveItem(0);}},doRemove:function(item,index,destroy){var me=this;me.items.remove(item);if(item.isInnerItem()){me.removeInner(item);}
me.onItemRemove(item,index,destroy);item.setParent(null);if(destroy){item.destroy();}},removeAll:function(destroy,everything){var items=this.items,ln=items.length,i=0,item;if(destroy===undefined){destroy=this.getAutoDestroy();}
everything=Boolean(everything);this.removingAll=true;for(;i<ln;i++){item=items.getAt(i);if(item&&(everything||item.isInnerItem())){this.doRemove(item,i,destroy);i--;ln--;}}
this.removingAll=false;return this;},getAt:function(index){return this.items.getAt(index);},removeAt:function(index){var item=this.getAt(index);if(item){this.remove(item);}
return this;},removeInnerAt:function(index){var item=this.getInnerItems()[index];if(item){this.remove(item);}
return this;},has:function(item){return this.getItems().indexOf(item)!=-1;},hasInnerItem:function(item){return this.innerItems.indexOf(item)!=-1;},indexOf:function(item){return this.getItems().indexOf(item);},insertInner:function(item,index){var items=this.getItems().items,innerItems=this.innerItems,currentInnerIndex=innerItems.indexOf(item),newInnerIndex=-1,nextSibling;if(currentInnerIndex!==-1){innerItems.splice(currentInnerIndex,1);}
if(typeof index=='number'){do{nextSibling=items[++index];}while(nextSibling&&!nextSibling.isInnerItem());if(nextSibling){newInnerIndex=innerItems.indexOf(nextSibling);innerItems.splice(newInnerIndex,0,item);}}
if(newInnerIndex===-1){innerItems.push(item);newInnerIndex=innerItems.length-1;}
if(currentInnerIndex!==-1){this.onInnerItemMove(item,newInnerIndex,currentInnerIndex);}
return this;},onInnerItemMove:Ext.emptyFn,removeInner:function(item){Ext.Array.remove(this.innerItems,item);return this;},insert:function(index,item){var me=this,i;if(Ext.isArray(item)){for(i=item.length-1;i>=0;i--){me.insert(index,item[i]);}
return me;}
item=this.factoryItem(item);this.doInsert(index,item);return item;},doInsert:function(index,item){var me=this,items=me.items,itemsLength=items.length,currentIndex,isInnerItem;isInnerItem=item.isInnerItem();if(index>itemsLength){index=itemsLength;}
if(items[index-1]===item){return me;}
currentIndex=me.indexOf(item);if(currentIndex!==-1){if(currentIndex<index){index-=1;}
items.removeAt(currentIndex);}
else{item.setParent(me);}
items.insert(index,item);if(isInnerItem){me.insertInner(item,index);}
if(currentIndex!==-1){me.onItemMove(item,index,currentIndex);}
else{me.onItemAdd(item,index);}},insertFirst:function(item){return this.insert(0,item);},insertLast:function(item){return this.insert(this.getItems().length,item);},insertBefore:function(item,relativeToItem){var index=this.indexOf(relativeToItem);if(index!==-1){this.insert(index,item);}
return this;},insertAfter:function(item,relativeToItem){var index=this.indexOf(relativeToItem);if(index!==-1){this.insert(index+1,item);}
return this;},onItemAdd:function(item,index){this.doItemLayoutAdd(item,index);if(this.initialized){this.fireEvent('add',this,item,index);}},doItemLayoutAdd:function(item,index){var layout=this.getLayout();if(this.isRendered()&&item.setRendered(true)){item.fireAction('renderedchange',[this,item,true],'onItemAdd',layout,{args:[item,index]});}
else{layout.onItemAdd(item,index);}},onItemRemove:function(item,index){this.doItemLayoutRemove(item,index);this.fireEvent('remove',this,item,index);},doItemLayoutRemove:function(item,index){var layout=this.getLayout();if(this.isRendered()&&item.setRendered(false)){item.fireAction('renderedchange',[this,item,false],'onItemRemove',layout,{args:[item,index]});}
else{layout.onItemRemove(item,index);}},onItemMove:function(item,toIndex,fromIndex){if(item.isDocked()){item.setDocked(null);}
this.doItemLayoutMove(item,toIndex,fromIndex);this.fireEvent('move',this,item,toIndex,fromIndex);},doItemLayoutMove:function(item,toIndex,fromIndex){this.getLayout().onItemMove(item,toIndex,fromIndex);},onItemCenteredChange:function(item,centered){if(!item.isFloating()&&!item.isDocked()){if(centered){this.removeInner(item);}
else{this.insertInner(item,this.indexOf(item));}}
this.getLayout().onItemCenteredChange(item,centered);},onItemFloatingChange:function(item,floating){if(!item.isCentered()&&!item.isDocked()){if(floating){this.removeInner(item);}
else{this.insertInner(item,this.indexOf(item));}}
this.getLayout().onItemFloatingChange(item,floating);},onItemDockedChange:function(item,docked,oldDocked){if(!item.isCentered()&&!item.isFloating()){if(docked){this.removeInner(item);}
else{this.insertInner(item,this.indexOf(item));}}
this.getLayout().onItemDockedChange(item,docked,oldDocked);},getInnerItems:function(){return this.innerItems;},getDockedItems:function(){var items=this.getItems().items,dockedItems=[],ln=items.length,item,i;for(i=0;i<ln;i++){item=items[i];if(item.isDocked()){dockedItems.push(item);}}
return dockedItems;},applyActiveItem:function(activeItem,currentActiveItem){var innerItems=this.getInnerItems();this.getItems();if(!activeItem&&innerItems.length===0){return 0;}
else if(typeof activeItem=='number'){activeItem=Math.max(0,Math.min(activeItem,innerItems.length-1));activeItem=innerItems[activeItem];if(activeItem){return activeItem;}
else if(currentActiveItem){return null;}}
else if(activeItem){if(!activeItem.isComponent){activeItem=this.factoryItem(activeItem);}
if(!activeItem.isInnerItem()){Ext.Logger.error("Setting activeItem to be a non-inner item");}
if(!this.has(activeItem)){this.add(activeItem);}
return activeItem;}},animateActiveItem:function(activeItem,animation){var layout=this.getLayout(),defaultAnimation;if(this.activeItemAnimation){this.activeItemAnimation.destroy();}
this.activeItemAnimation=animation=new Ext.fx.layout.Card(animation);if(animation&&layout.isCard){animation.setLayout(layout);defaultAnimation=layout.getAnimation();if(defaultAnimation){defaultAnimation.disable();animation.on('animationend',function(){defaultAnimation.enable();animation.destroy();},this);}}
return this.setActiveItem(activeItem);},doSetActiveItem:function(newActiveItem,oldActiveItem){if(oldActiveItem){oldActiveItem.fireEvent('deactivate',oldActiveItem,this,newActiveItem);}
if(newActiveItem){newActiveItem.fireEvent('activate',newActiveItem,this,oldActiveItem);}},setRendered:function(rendered){if(this.callParent(arguments)){var items=this.items.items,i,ln;for(i=0,ln=items.length;i<ln;i++){items[i].setRendered(rendered);}
return true;}
return false;},getScrollableBehavior:function(){var behavior=this.scrollableBehavior;if(!behavior){behavior=this.scrollableBehavior=new Ext.behavior.Scrollable(this);}
return behavior;},applyScrollable:function(config){this.getScrollableBehavior().setConfig(config);},getScrollable:function(){return this.getScrollableBehavior().getScrollView();},getRefItems:function(deep){var items=this.getItems().items.slice(),ln=items.length,i,item;if(deep){for(i=0;i<ln;i++){item=items[i];if(item.getRefItems){items=items.concat(item.getRefItems(true));}}}
return items;},getComponent:function(component){if(Ext.isObject(component)){component=component.getItemId();}
return this.getItems().get(component);},getDockedComponent:function(component){if(Ext.isObject(component)){component=component.getItemId();}
var dockedItems=this.getDockedItems(),ln=dockedItems.length,item,i;if(Ext.isNumber(component)){return dockedItems[component];}
for(i=0;i<ln;i++){item=dockedItems[i];if(item.id==component){return item;}}
return false;},query:function(selector){return Ext.ComponentQuery.query(selector,this);},child:function(selector){return this.query('> '+selector)[0]||null;},down:function(selector){return this.query(selector)[0]||null;},onClassExtended:function(Class,members){if('onAdd'in members||'onRemove'in members){throw new Error("["+Class.$className+"] 'onAdd()' and 'onRemove()' methods "+"no longer exist in Ext.Container, please use 'onItemAdd()' "+"and 'onItemRemove()' instead }");}},destroy:function(){var modal=this.getModal();if(modal){modal.destroy();}
this.removeAll(true,true);Ext.destroy(this.getScrollable(),this.bodyElement);this.callParent();}},function(){this.addMember('defaultItemClass',this);Ext.deprecateClassMethod(this,'addAll','add');Ext.deprecateClassMethod(this,'removeDocked','remove');this.override({constructor:function(config){config=config||{};var dockedItems=config.dockedItems,i,ln,item;if(config.scroll){Ext.Logger.deprecate("'scroll' config is deprecated, please use 'scrollable' instead.",this);config.scrollable=config.scroll;delete config.scroll;}
this.callParent(arguments);if(dockedItems){Ext.Logger.deprecate("'dockedItems' config is deprecated, please add all docked items inside the 'items' config with a 'docked' property indicating the docking position instead, i.e { /*...*/ docked: 'top' /*...*/ }");dockedItems=Ext.Array.from(dockedItems);for(i=0,ln=dockedItems.length;i<ln;i++){item=dockedItems[i];if('dock'in item){Ext.Logger.deprecate("'dock' config for docked items is deprecated, please use 'docked' instead");item.docked=item.dock;}}
this.add(dockedItems);}},add:function(){var args=arguments;if(args.length>1){if(typeof args[0]=='number'){Ext.Logger.deprecate("add(index, item) method signature is deprecated, please use insert(index, item) instead");return this.insert(args[0],args[1]);}
Ext.Logger.deprecate("Passing items as multiple arguments is deprecated, please use one single array of items instead");args=[Array.prototype.slice.call(args)];}
return this.callParent(args);},doAdd:function(item){var docked=item.getDocked(),overlay=item.overlay,position;if(overlay&&docked){Ext.Logger.deprecate("'overlay' config is deprecated on docked items, please set the top/left/right/bottom configurations instead.",this);if(docked=="top"){position={top:0,bottom:'auto',left:0,right:0};}else if(docked=="bottom"){position={top:null,bottom:0,left:0,right:0};}
if(position){item.setDocked(false);item.setTop(position.top);item.setBottom(position.bottom);item.setLeft(position.left);item.setRight(position.right);}}
return this.callOverridden(arguments);},applyDefaults:function(defaults){if(typeof defaults=='function'){Ext.Logger.deprecate("Passing a function as 'defaults' is deprecated. To add custom logics when "+"'defaults' is applied to each item, have your own factoryItem() method in your sub-class instead");}
return this.callParent(arguments);},factoryItemWithDefaults:function(item){var defaults=this.getDefaults(),customDefaults,ret;if(typeof defaults=='function'){customDefaults=defaults.call(this,item);}
if(typeof item=='string'){Ext.Logger.deprecate("Passing a string id of item ('"+item+"') is deprecated, please pass a reference to that item instead");item=Ext.getCmp(item);}
if(customDefaults){this._defaults=customDefaults;}
ret=this.callParent([item]);if(customDefaults){this._defaults=defaults;}
return ret;},applyMasked:function(masked){if(Ext.isObject(masked)&&!masked.isInstance&&'message'in masked&&!('xtype'in masked)&&!('xclass'in masked)){masked.xtype='loadmask';Ext.Logger.deprecate("Using a 'message' config without specify an 'xtype' or 'xclass' will no longer implicitly set 'xtype' to 'loadmask'. Please set that explicitly.");}
return this.callOverridden(arguments);}});Ext.deprecateClassMethod(this,'setMask','setMasked');});Ext.define('Ext.Toolbar',{extend:'Ext.Container',xtype:'toolbar',requires:['Ext.Button','Ext.Title','Ext.Spacer'],isToolbar:true,config:{baseCls:Ext.baseCSSPrefix+'toolbar',ui:'dark',title:null,defaultType:'button',layout:{type:'hbox',align:'center'}},constructor:function(config){config=config||{};if(config.docked=="left"||config.docked=="right"){config.layout={type:'vbox',align:'stretch'};}
this.callParent([config]);},applyTitle:function(title){if(typeof title=='string'){title={title:title,centered:true};}
return Ext.factory(title,Ext.Title,this.getTitle());},updateTitle:function(newTitle,oldTitle){if(newTitle){this.add(newTitle);this.getLayout().setItemFlex(newTitle,1);}
if(oldTitle){oldTitle.destroy();}},showTitle:function(){var title=this.getTitle();if(title){title.show();}},hideTitle:function(){var title=this.getTitle();if(title){title.hide();}}},function(){Ext.deprecateProperty(this,'titleCls',null,"Ext.Toolbar.titleCls has been removed. Use #cls config of title instead.");});Ext.define('Ext.Panel',{extend:'Ext.Container',requires:['Ext.util.LineSegment'],alternateClassName:'Ext.lib.Panel',xtype:'panel',isPanel:true,config:{baseCls:Ext.baseCSSPrefix+'panel',bodyPadding:null,bodyMargin:null,bodyBorder:null},getElementConfig:function(){var config=this.callParent();config.children.push({reference:'tipElement',className:'x-anchor',hidden:true});return config;},applyBodyPadding:function(bodyPadding){if(bodyPadding===true){bodyPadding=5;}
if(bodyPadding){bodyPadding=Ext.dom.Element.unitizeBox(bodyPadding);}
return bodyPadding;},updateBodyPadding:function(newBodyPadding){this.element.setStyle('padding',newBodyPadding);},applyBodyMargin:function(bodyMargin){if(bodyMargin===true){bodyMargin=5;}
if(bodyMargin){bodyMargin=Ext.dom.Element.unitizeBox(bodyMargin);}
return bodyMargin;},updateBodyMargin:function(newBodyMargin){this.element.setStyle('margin',newBodyMargin);},applyBodyBorder:function(bodyBorder){if(bodyBorder===true){bodyBorder=1;}
if(bodyBorder){bodyBorder=Ext.dom.Element.unitizeBox(bodyBorder);}
return bodyBorder;},updateBodyBorder:function(newBodyBorder){this.element.setStyle('border-width',newBodyBorder);},alignTo:function(component){var tipElement=this.tipElement;tipElement.hide();if(this.currentTipPosition){tipElement.removeCls('x-anchor-'+this.currentTipPosition);}
this.callParent(arguments);var LineSegment=Ext.util.LineSegment,alignToElement=component.isComponent?component.renderElement:component,element=this.renderElement,alignToBox=alignToElement.getPageBox(),box=element.getPageBox(),left=box.left,top=box.top,right=box.right,bottom=box.bottom,centerX=left+(box.width/2),centerY=top+(box.height/2),leftTopPoint={x:left,y:top},rightTopPoint={x:right,y:top},leftBottomPoint={x:left,y:bottom},rightBottomPoint={x:right,y:bottom},boxCenterPoint={x:centerX,y:centerY},alignToCenterX=alignToBox.left+(alignToBox.width/2),alignToCenterY=alignToBox.top+(alignToBox.height/2),alignToBoxCenterPoint={x:alignToCenterX,y:alignToCenterY},centerLineSegment=new LineSegment(boxCenterPoint,alignToBoxCenterPoint),offsetLeft=0,offsetTop=0,tipSize,tipWidth,tipHeight,tipPosition,tipX,tipY;tipElement.setVisibility(false);tipElement.show();tipSize=tipElement.getSize();tipWidth=tipSize.width;tipHeight=tipSize.height;if(centerLineSegment.intersects(new LineSegment(leftTopPoint,rightTopPoint))){tipX=Math.min(Math.max(alignToCenterX,left),right-(tipWidth/2));tipY=top;offsetTop=tipHeight+10;tipPosition='top';}
else if(centerLineSegment.intersects(new LineSegment(leftTopPoint,leftBottomPoint))){tipX=left;tipY=Math.min(Math.max(alignToCenterY+(tipWidth/2),top),bottom);offsetLeft=tipHeight+10;tipPosition='left';}
else if(centerLineSegment.intersects(new LineSegment(leftBottomPoint,rightBottomPoint))){tipX=Math.min(Math.max(alignToCenterX,left),right-(tipWidth/2));tipY=bottom;offsetTop=-tipHeight-10;tipPosition='bottom';}
else if(centerLineSegment.intersects(new LineSegment(rightTopPoint,rightBottomPoint))){tipX=right;tipY=Math.min(Math.max(alignToCenterY-(tipWidth/2),top),bottom);offsetLeft=-tipHeight-10;tipPosition='right';}
if(tipX||tipY){this.currentTipPosition=tipPosition;tipElement.addCls('x-anchor-'+tipPosition);tipElement.setLeft(tipX-left);tipElement.setTop(tipY-top);tipElement.setVisibility(true);this.setLeft(this.getLeft()+offsetLeft);this.setTop(this.getTop()+offsetTop);}}});Ext.define('Ext.Sheet',{extend:'Ext.Panel',xtype:'sheet',requires:['Ext.fx.Animation'],config:{baseCls:Ext.baseCSSPrefix+'sheet',modal:true,centered:true,stretchX:null,stretchY:null,enter:'bottom',exit:'bottom',showAnimation:!Ext.os.is.Android2?{type:'slideIn',duration:250,easing:'ease-out'}:null,hideAnimation:!Ext.os.is.Android2?{type:'slideOut',duration:250,easing:'ease-in'}:null},applyHideAnimation:function(config){var exit=this.getExit(),direction=exit;if(exit===null){return null;}
if(config===true){config={type:'slideOut'};}
if(Ext.isString(config)){config={type:config};}
var anim=Ext.factory(config,Ext.fx.Animation);if(anim){if(exit=='bottom'){direction='down';}
if(exit=='top'){direction='up';}
anim.setDirection(direction);}
return anim;},applyShowAnimation:function(config){var enter=this.getEnter(),direction=enter;if(enter===null){return null;}
if(config===true){config={type:'slideIn'};}
if(Ext.isString(config)){config={type:config};}
var anim=Ext.factory(config,Ext.fx.Animation);if(anim){if(enter=='bottom'){direction='down';}
if(enter=='top'){direction='up';}
anim.setBefore({display:null});anim.setReverse(true);anim.setDirection(direction);}
return anim;},updateStretchX:function(newStretchX){this.getLeft();this.getRight();if(newStretchX){this.setLeft(0);this.setRight(0);}},updateStretchY:function(newStretchY){this.getTop();this.getBottom();if(newStretchY){this.setTop(0);this.setBottom(0);}}});Ext.define('Ext.MessageBox',{extend:'Ext.Sheet',requires:['Ext.Toolbar','Ext.field.Text','Ext.field.TextArea'],config:{ui:'dark',baseCls:Ext.baseCSSPrefix+'msgbox',iconCls:null,showAnimation:{type:'popIn',duration:250,easing:'ease-out'},hideAnimation:{type:'popOut',duration:250,easing:'ease-out'},zIndex:10,defaultTextHeight:75,title:null,buttons:null,message:null,prompt:null,layout:{type:'vbox',pack:'center'}},statics:{OK:{text:'OK',itemId:'ok',ui:'action'},YES:{text:'Yes',itemId:'yes',ui:'action'},NO:{text:'No',itemId:'no'},CANCEL:{text:'Cancel',itemId:'cancel'},INFO:Ext.baseCSSPrefix+'msgbox-info',WARNING:Ext.baseCSSPrefix+'msgbox-warning',QUESTION:Ext.baseCSSPrefix+'msgbox-question',ERROR:Ext.baseCSSPrefix+'msgbox-error',OKCANCEL:[{text:'Cancel',itemId:'cancel'},{text:'OK',itemId:'ok',ui:'action'}],YESNOCANCEL:[{text:'Cancel',itemId:'cancel'},{text:'No',itemId:'no'},{text:'Yes',itemId:'yes',ui:'action'}],YESNO:[{text:'No',itemId:'no'},{text:'Yes',itemId:'yes',ui:'action'}]},constructor:function(config){config=config||{};if(config.hasOwnProperty('promptConfig')){Ext.Logger.deprecate("'promptConfig' config is deprecated, please use 'prompt' config instead",this);Ext.applyIf(config,{prompt:config.promptConfig});delete config.promptConfig;}
if(config.hasOwnProperty('multiline')||config.hasOwnProperty('multiLine')){config.prompt=config.prompt||{};Ext.applyIf(config.prompt,{multiLine:config.multiline||config.multiLine});delete config.multiline;delete config.multiLine;}
this.defaultAllowedConfig={};var allowedConfigs=['ui','showAnimation','hideAnimation','title','message','prompt','iconCls','buttons','defaultTextHeight'],ln=allowedConfigs.length,i,allowedConfig;for(i=0;i<ln;i++){allowedConfig=allowedConfigs[i];this.defaultAllowedConfig[allowedConfig]=this.defaultConfig[allowedConfig];}
this.callParent([config]);},applyTitle:function(config){if(typeof config=="string"){config={title:config};}
Ext.applyIf(config,{docked:'top',cls:this.getBaseCls()+'-title'});return Ext.factory(config,Ext.Toolbar,this.getTitle());},updateTitle:function(newTitle){if(newTitle){this.add(newTitle);}},updateButtons:function(newButtons){var me=this;if(newButtons){if(me.buttonsToolbar){me.buttonsToolbar.removeAll();me.buttonsToolbar.setItems(newButtons);}else{me.buttonsToolbar=Ext.create('Ext.Toolbar',{docked:'bottom',defaultType:'button',layout:{type:'hbox',pack:'center'},ui:me.getUi(),cls:me.getBaseCls()+'-buttons',items:newButtons});me.add(me.buttonsToolbar);}}},applyMessage:function(config){config={html:config,cls:this.getBaseCls()+'-text'};return Ext.factory(config,Ext.Component,this._message);},updateMessage:function(newMessage){if(newMessage){this.add(newMessage);}},getMessage:function(){if(this._message){return this._message.getHtml();}
return null;},applyIconCls:function(config){config={xtype:'component',docked:'left',width:40,height:40,baseCls:Ext.baseCSSPrefix+'icon',hidden:(config)?false:true,cls:config};return Ext.factory(config,Ext.Component,this._iconCls);},updateIconCls:function(newIconCls,oldIconCls){var me=this;this.getTitle();this.getButtons();if(newIconCls&&!oldIconCls){this.add(newIconCls);}else{this.remove(oldIconCls);}},getIconCls:function(){var icon=this._iconCls,iconCls;if(icon){iconCls=icon.getCls();return(iconCls)?iconCls[0]:null;}
return null;},applyPrompt:function(prompt){if(prompt){var config={label:false};if(Ext.isObject(prompt)){Ext.apply(config,prompt);}
if(config.multiLine){config.height=Ext.isNumber(config.multiLine)?parseFloat(config.multiLine):this.getDefaultTextHeight();return Ext.factory(config,Ext.field.TextArea,this.getPrompt());}else{return Ext.factory(config,Ext.field.Text,this.getPrompt());}}
return prompt;},updatePrompt:function(newPrompt,oldPrompt){if(newPrompt){this.add(newPrompt);}
if(oldPrompt){this.remove(oldPrompt);}},onClick:function(button){if(button){var config=button.config.userConfig||{},initialConfig=button.getInitialConfig(),prompt=this.getPrompt();if(typeof config.fn=='function'){this.on({hiddenchange:function(){config.fn.call(config.scope||null,initialConfig.itemId||initialConfig.text,prompt?prompt.getValue():null,config);},single:true,scope:this});}
if(config.input){config.input.dom.blur();}}
this.hide();},show:function(initialConfig){if(!this.getParent()&&Ext.Viewport){Ext.Viewport.add(this);}
if(!initialConfig){return this.callParent();}
var config=Ext.Object.merge({},{value:''},initialConfig);var buttons=initialConfig.buttons||Ext.MessageBox.OK||[],buttonBarItems=[],userConfig=initialConfig;Ext.each(buttons,function(buttonConfig){if(!buttonConfig){return;}
buttonBarItems.push(Ext.apply({userConfig:userConfig,scope:this,handler:'onClick'},buttonConfig));},this);config.buttons=buttonBarItems;if(config.promptConfig){Ext.Logger.deprecate("'promptConfig' config is deprecated, please use 'prompt' config instead",this);}
config.prompt=(config.promptConfig||config.prompt)||null;if(config.multiLine){config.prompt=config.prompt||{};config.prompt.multiLine=config.multiLine;delete config.multiLine;}
config=Ext.merge({},this.defaultAllowedConfig,config);this.setConfig(config);var prompt=this.getPrompt();if(prompt){prompt.setValue(initialConfig.value||'');}
this.callParent();return this;},alert:function(title,message,fn,scope){return this.show({title:title,message:message,buttons:Ext.MessageBox.OK,promptConfig:false,fn:function(buttonId){if(fn){fn.call(scope,buttonId);}},scope:scope});},confirm:function(title,message,fn,scope){return this.show({title:title,message:message,buttons:Ext.MessageBox.YESNO,promptConfig:false,scope:scope,fn:function(button){if(fn){fn.call(scope,button);}}});},prompt:function(title,message,fn,scope,multiLine,value,prompt){return this.show({title:title,message:message,buttons:Ext.MessageBox.OKCANCEL,scope:scope,prompt:prompt||true,multiLine:multiLine,value:value,fn:function(button,inputValue){if(fn){fn.call(scope,button,inputValue);}}});}},function(MessageBox){this.override({setIcon:function(iconCls,doLayout){Ext.Logger.deprecate("Ext.MessageBox#setIcon is deprecated, use setIconCls instead",2);this.setIconCls(iconCls);return this;},updateText:function(text){Ext.Logger.deprecate("Ext.MessageBox#updateText is deprecated, use setMessage instead",2);this.setMessage(text);return this;}});Ext.onSetup(function(){Ext.Msg=new MessageBox;});});Ext.define('Ext.event.Touch',{extend:'Ext.event.Dom',requires:['Ext.util.Point'],constructor:function(event,info){if(info){this.set(info);}
this.touchesMap={};this.changedTouches=this.cloneTouches(event.changedTouches);this.touches=this.cloneTouches(event.touches);this.targetTouches=this.cloneTouches(event.targetTouches);return this.callParent([event]);},clone:function(){return new this.self(this);},setTargets:function(targetsMap){this.doSetTargets(this.changedTouches,targetsMap);this.doSetTargets(this.touches,targetsMap);this.doSetTargets(this.targetTouches,targetsMap);},doSetTargets:function(touches,targetsMap){var i,ln,touch,identifier,targets;for(i=0,ln=touches.length;i<ln;i++){touch=touches[i];identifier=touch.identifier;targets=targetsMap[identifier];if(targets){touch.targets=targets;}}},cloneTouches:function(touches){var map=this.touchesMap,clone=[],lastIdentifier=null,i,ln,touch,identifier;for(i=0,ln=touches.length;i<ln;i++){touch=touches[i];identifier=touch.identifier;if(lastIdentifier!==null&&identifier===lastIdentifier){identifier++;}
lastIdentifier=identifier;if(!map[identifier]){map[identifier]={pageX:touch.pageX,pageY:touch.pageY,identifier:identifier,target:touch.target,timeStamp:touch.timeStamp,point:Ext.util.Point.fromTouch(touch),targets:touch.targets};}
clone[i]=map[identifier];}
return clone;}});Ext.define('Ext.event.publisher.TouchGesture',{extend:'Ext.event.publisher.Dom',requires:['Ext.util.Point','Ext.event.Touch'],handledEvents:['touchstart','touchmove','touchend','touchcancel'],moveEventName:'touchmove',config:{moveThrottle:1,buffering:{enabled:false,interval:10},recognizers:{}},currentTouchesCount:0,constructor:function(config){this.processEvents=Ext.Function.bind(this.processEvents,this);this.eventProcessors={touchstart:this.onTouchStart,touchmove:this.onTouchMove,touchend:this.onTouchEnd,touchcancel:this.onTouchEnd};this.eventToRecognizerMap={};this.activeRecognizers=[];this.currentRecognizers=[];this.currentTargets={};this.currentTouches={};this.buffer=[];this.initConfig(config);return this.callParent();},applyBuffering:function(buffering){if(buffering.enabled===true){this.bufferTimer=setInterval(this.processEvents,buffering.interval);}
else{clearInterval(this.bufferTimer);}
return buffering;},applyRecognizers:function(recognizers){var i,recognizer;for(i in recognizers){if(recognizers.hasOwnProperty(i)){recognizer=recognizers[i];if(recognizer){this.registerRecognizer(recognizer);}}}
return recognizers;},handles:function(eventName){return this.callParent(arguments)||this.eventToRecognizerMap.hasOwnProperty(eventName);},doesEventBubble:function(){return true;},eventLogs:[],onEvent:function(e){var buffering=this.getBuffering();e=new Ext.event.Touch(e);if(buffering.enabled){this.buffer.push(e);}
else{this.processEvent(e);}},processEvents:function(){var buffer=this.buffer,ln=buffer.length,moveEvents=[],events,event,i;if(ln>0){events=buffer.slice(0);buffer.length=0;for(i=0;i<ln;i++){event=events[i];if(event.type===this.moveEventName){moveEvents.push(event);}
else{if(moveEvents.length>0){this.processEvent(this.mergeEvents(moveEvents));moveEvents.length=0;}
this.processEvent(event);}}
if(moveEvents.length>0){this.processEvent(this.mergeEvents(moveEvents));moveEvents.length=0;}}},mergeEvents:function(events){var changedTouchesLists=[],ln=events.length,i,event,targetEvent;targetEvent=events[ln-1];if(ln===1){return targetEvent;}
for(i=0;i<ln;i++){event=events[i];changedTouchesLists.push(event.changedTouches);}
targetEvent.changedTouches=this.mergeTouchLists(changedTouchesLists);return targetEvent;},mergeTouchLists:function(touchLists){var touches={},list=[],i,ln,touchList,j,subLn,touch,identifier;for(i=0,ln=touchLists.length;i<ln;i++){touchList=touchLists[i];for(j=0,subLn=touchList.length;j<subLn;j++){touch=touchList[j];identifier=touch.identifier;touches[identifier]=touch;}}
for(identifier in touches){if(touches.hasOwnProperty(identifier)){list.push(touches[identifier]);}}
return list;},registerRecognizer:function(recognizer){var map=this.eventToRecognizerMap,activeRecognizers=this.activeRecognizers,handledEvents=recognizer.getHandledEvents(),i,ln,eventName;recognizer.setOnRecognized(this.onRecognized);recognizer.setCallbackScope(this);for(i=0,ln=handledEvents.length;i<ln;i++){eventName=handledEvents[i];map[eventName]=recognizer;}
activeRecognizers.push(recognizer);return this;},onRecognized:function(eventName,e,touches,info){var targetGroups=[],ln=touches.length,targets,i,touch;if(ln===1){return this.publish(eventName,touches[0].targets,e,info);}
for(i=0;i<ln;i++){touch=touches[i];targetGroups.push(touch.targets);}
targets=this.getCommonTargets(targetGroups);this.publish(eventName,targets,e,info);},publish:function(eventName,targets,event,info){event.set(info);return this.callParent([eventName,targets,event]);},getCommonTargets:function(targetGroups){var firstTargetGroup=targetGroups[0],ln=targetGroups.length;if(ln===1){return firstTargetGroup;}
var commonTargets=[],i=1,target,targets,j;while(true){target=firstTargetGroup[firstTargetGroup.length-i];if(!target){return commonTargets;}
for(j=1;j<ln;j++){targets=targetGroups[j];if(targets[targets.length-i]!==target){return commonTargets;}}
commonTargets.unshift(target);i++;}
return commonTargets;},invokeRecognizers:function(methodName,e){var recognizers=this.activeRecognizers,ln=recognizers.length,i,recognizer;if(methodName==='onStart'){for(i=0;i<ln;i++){recognizers[i].isActive=true;}}
for(i=0;i<ln;i++){recognizer=recognizers[i];if(recognizer.isActive&&recognizer[methodName].call(recognizer,e)===false){recognizer.isActive=false;}}},getActiveRecognizers:function(){return this.activeRecognizers;},processEvent:function(e){this.eventProcessors[e.type].call(this,e);},onTouchStart:function(e){var currentTargets=this.currentTargets,currentTouches=this.currentTouches,currentTouchesCount=this.currentTouchesCount,changedTouches=e.changedTouches,touches=e.touches,touchesLn=touches.length,currentIdentifiers={},ln=changedTouches.length,i,touch,identifier,fakeEndEvent;currentTouchesCount+=ln;if(currentTouchesCount>touchesLn){for(i=0;i<touchesLn;i++){touch=touches[i];identifier=touch.identifier;currentIdentifiers[identifier]=true;}
for(identifier in currentTouches){if(currentTouches.hasOwnProperty(identifier)){if(!currentIdentifiers[identifier]){currentTouchesCount--;fakeEndEvent=e.clone();touch=currentTouches[identifier];touch.targets=this.getBubblingTargets(this.getElementTarget(touch.target));fakeEndEvent.changedTouches=[touch];this.onTouchEnd(fakeEndEvent);}}}
if(currentTouchesCount>touchesLn){return;}}
for(i=0;i<ln;i++){touch=changedTouches[i];identifier=touch.identifier;if(!currentTouches.hasOwnProperty(identifier)){this.currentTouchesCount++;}
currentTouches[identifier]=touch;currentTargets[identifier]=this.getBubblingTargets(this.getElementTarget(touch.target));}
e.setTargets(currentTargets);for(i=0;i<ln;i++){touch=changedTouches[i];this.publish('touchstart',touch.targets,e,{touch:touch});}
if(!this.isStarted){this.isStarted=true;this.invokeRecognizers('onStart',e);}
this.invokeRecognizers('onTouchStart',e);},onTouchMove:function(e){if(!this.isStarted){return;}
var currentTargets=this.currentTargets,currentTouches=this.currentTouches,moveThrottle=this.getMoveThrottle(),changedTouches=e.changedTouches,stillTouchesCount=0,i,ln,touch,point,oldPoint,identifier;e.setTargets(currentTargets);for(i=0,ln=changedTouches.length;i<ln;i++){touch=changedTouches[i];identifier=touch.identifier;point=touch.point;oldPoint=currentTouches[identifier].point;if(moveThrottle&&point.isCloseTo(oldPoint,moveThrottle)){stillTouchesCount++;continue;}
currentTouches[identifier]=touch;this.publish('touchmove',touch.targets,e,{touch:touch});}
if(stillTouchesCount<ln){this.invokeRecognizers('onTouchMove',e);}},onTouchEnd:function(e){if(!this.isStarted){return;}
var currentTargets=this.currentTargets,currentTouches=this.currentTouches,changedTouches=e.changedTouches,ln=changedTouches.length,isEnded,identifier,i,touch;e.setTargets(currentTargets);this.currentTouchesCount-=ln;isEnded=(this.currentTouchesCount===0);if(isEnded){this.isStarted=false;}
for(i=0;i<ln;i++){touch=changedTouches[i];identifier=touch.identifier;delete currentTouches[identifier];delete currentTargets[identifier];this.publish('touchend',touch.targets,e,{touch:touch});}
this.invokeRecognizers('onTouchEnd',e);if(isEnded){this.invokeRecognizers('onEnd',e);}}},function(){if(!Ext.feature.has.Touch){this.override({moveEventName:'mousemove',map:{mouseToTouch:{mousedown:'touchstart',mousemove:'touchmove',mouseup:'touchend'},touchToMouse:{touchstart:'mousedown',touchmove:'mousemove',touchend:'mouseup'}},attachListener:function(eventName){eventName=this.map.touchToMouse[eventName];if(!eventName){return;}
return this.callOverridden([eventName]);},lastEventType:null,onEvent:function(e){if('button'in e&&e.button!==0){return;}
var type=e.type,touchList=[e];if(type==='mousedown'&&this.lastEventType&&this.lastEventType!=='mouseup'){var fixedEvent=document.createEvent("MouseEvent");fixedEvent.initMouseEvent('mouseup',e.bubbles,e.cancelable,document.defaultView,e.detail,e.screenX,e.screenY,e.clientX,e.clientY,e.ctrlKey,e.altKey,e.shiftKey,e.metaKey,e.metaKey,e.button,e.relatedTarget);this.onEvent(fixedEvent);}
if(type!=='mousemove'){this.lastEventType=type;}
e.identifier=1;e.touches=(type!=='mouseup')?touchList:[];e.targetTouches=(type!=='mouseup')?touchList:[];e.changedTouches=touchList;return this.callOverridden([e]);},processEvent:function(e){this.eventProcessors[this.map.mouseToTouch[e.type]].call(this,e);}});}});Ext.define('Ext.event.recognizer.Recognizer',{mixins:['Ext.mixin.Identifiable'],handledEvents:[],config:{onRecognized:Ext.emptyFn,onFailed:Ext.emptyFn,callbackScope:null},constructor:function(config){this.initConfig(config);return this;},getHandledEvents:function(){return this.handledEvents;},onStart:Ext.emptyFn,onEnd:Ext.emptyFn,fail:function(){this.getOnFailed().apply(this.getCallbackScope(),arguments);return false;},fire:function(){this.getOnRecognized().apply(this.getCallbackScope(),arguments);}});Ext.define('Ext.event.recognizer.Touch',{extend:'Ext.event.recognizer.Recognizer',onTouchStart:Ext.emptyFn,onTouchMove:Ext.emptyFn,onTouchEnd:Ext.emptyFn});Ext.define('Ext.event.recognizer.SingleTouch',{extend:'Ext.event.recognizer.Touch',inheritableStatics:{NOT_SINGLE_TOUCH:0x01,TOUCH_MOVED:0x02},onTouchStart:function(e){if(e.touches.length>1){return this.fail(this.self.NOT_SINGLE_TOUCH);}}});Ext.define('Ext.event.recognizer.Drag',{extend:'Ext.event.recognizer.SingleTouch',isStarted:false,startPoint:null,previousPoint:null,lastPoint:null,handledEvents:['dragstart','drag','dragend'],onTouchStart:function(e){var startTouches,startTouch;if(this.callParent(arguments)===false){if(this.isStarted&&this.lastMoveEvent!==null){this.onTouchEnd(this.lastMoveEvent);}
return false;}
this.startTouches=startTouches=e.changedTouches;this.startTouch=startTouch=startTouches[0];this.startPoint=startTouch.point;},onTouchMove:function(e){var touches=e.changedTouches,touch=touches[0],point=touch.point,time=e.time;if(this.lastPoint){this.previousPoint=this.lastPoint;}
if(this.lastTime){this.previousTime=this.lastTime;}
this.lastTime=time;this.lastPoint=point;this.lastMoveEvent=e;if(!this.isStarted){this.isStarted=true;this.startTime=time;this.previousTime=time;this.previousPoint=this.startPoint;this.fire('dragstart',e,this.startTouches,this.getInfo(e,this.startTouch));}
else{this.fire('drag',e,touches,this.getInfo(e,touch));}},onTouchEnd:function(e){if(this.isStarted){var touches=e.changedTouches,touch=touches[0],point=touch.point;this.isStarted=false;this.lastPoint=point;this.fire('dragend',e,touches,this.getInfo(e,touch));this.startTime=0;this.previousTime=0;this.lastTime=0;this.startPoint=null;this.previousPoint=null;this.lastPoint=null;this.lastMoveEvent=null;}},getInfo:function(e,touch){var time=e.time,startPoint=this.startPoint,previousPoint=this.previousPoint,startTime=this.startTime,previousTime=this.previousTime,point=this.lastPoint,deltaX=point.x-startPoint.x,deltaY=point.y-startPoint.y,info={touch:touch,startX:startPoint.x,startY:startPoint.y,previousX:previousPoint.x,previousY:previousPoint.y,pageX:point.x,pageY:point.y,deltaX:deltaX,deltaY:deltaY,absDeltaX:Math.abs(deltaX),absDeltaY:Math.abs(deltaY),previousDeltaX:point.x-previousPoint.x,previousDeltaY:point.y-previousPoint.y,time:time,startTime:startTime,previousTime:previousTime,deltaTime:time-startTime,previousDeltaTime:time-previousTime};return info;}});Ext.define('Ext.event.recognizer.Tap',{handledEvents:['tap'],extend:'Ext.event.recognizer.SingleTouch',onTouchMove:function(){return this.fail(this.self.TOUCH_MOVED);},onTouchEnd:function(e){var touch=e.changedTouches[0];this.fire('tap',e,[touch]);}},function(){this.override({handledEvents:['tap','tapstart','tapcancel'],onTouchStart:function(e){if(this.callOverridden(arguments)===false){return false;}
this.fire('tapstart',e,[e.changedTouches[0]]);},onTouchMove:function(e){this.fire('tapcancel',e,[e.changedTouches[0]]);return this.callOverridden(arguments);}});});Ext.define('Ext.event.recognizer.DoubleTap',{extend:'Ext.event.recognizer.SingleTouch',config:{maxDuration:300},handledEvents:['singletap','doubletap'],singleTapTimer:null,onTouchStart:function(e){if(this.callParent(arguments)===false){return false;}
this.startTime=e.time;clearTimeout(this.singleTapTimer);},onTouchMove:function(){return this.fail(this.self.TOUCH_MOVED);},onEnd:function(e){var me=this,maxDuration=this.getMaxDuration(),touch=e.changedTouches[0],time=e.time,lastTapTime=this.lastTapTime,duration;this.lastTapTime=time;if(lastTapTime){duration=time-lastTapTime;if(duration<=maxDuration){this.lastTapTime=0;this.fire('doubletap',e,[touch],{touch:touch,duration:duration});return;}}
if(time-this.startTime>maxDuration){this.fireSingleTap(e,touch);}
else{this.singleTapTimer=setTimeout(function(){me.fireSingleTap(e,touch);},maxDuration);}},fireSingleTap:function(e,touch){this.fire('singletap',e,[touch],{touch:touch});}});Ext.define('Ext.event.recognizer.LongPress',{extend:'Ext.event.recognizer.SingleTouch',inheritableStatics:{DURATION_NOT_ENOUGH:0x20},config:{minDuration:1000},handledEvents:['longpress'],fireLongPress:function(e){var touch=e.changedTouches[0];this.fire('longpress',e,[touch],{touch:touch,duration:this.getMinDuration()});this.isLongPress=true;},onTouchStart:function(e){var me=this;if(this.callParent(arguments)===false){return false;}
this.isLongPress=false;this.timer=setTimeout(function(){me.fireLongPress(e);},this.getMinDuration());},onTouchMove:function(){return this.fail(this.self.TOUCH_MOVED);},onTouchEnd:function(){if(!this.isLongPress){return this.fail(this.self.DURATION_NOT_ENOUGH);}},fail:function(){clearTimeout(this.timer);return this.callParent(arguments);}},function(){this.override({handledEvents:['longpress','taphold'],fire:function(eventName){if(eventName==='longpress'){var args=Array.prototype.slice.call(arguments);args[0]='taphold';this.fire.apply(this,args);}
return this.callOverridden(arguments);}});});Ext.define('Ext.event.recognizer.Swipe',{extend:'Ext.event.recognizer.SingleTouch',handledEvents:['swipe'],inheritableStatics:{MAX_OFFSET_EXCEEDED:0x10,MAX_DURATION_EXCEEDED:0x11,DISTANCE_NOT_ENOUGH:0x12},config:{minDistance:80,maxOffset:35,maxDuration:1000},onTouchStart:function(e){if(this.callParent(arguments)===false){return false;}
var touch=e.changedTouches[0];this.startTime=e.time;this.isHorizontal=true;this.isVertical=true;this.startX=touch.pageX;this.startY=touch.pageY;},onTouchMove:function(e){var touch=e.changedTouches[0],x=touch.pageX,y=touch.pageY,absDeltaX=Math.abs(x-this.startX),absDeltaY=Math.abs(y-this.startY),time=e.time;if(time-this.startTime>this.getMaxDuration()){return this.fail(this.self.MAX_DURATION_EXCEEDED);}
if(this.isVertical&&absDeltaX>this.getMaxOffset()){this.isVertical=false;}
if(this.isHorizontal&&absDeltaY>this.getMaxOffset()){this.isHorizontal=false;}
if(!this.isHorizontal&&!this.isVertical){return this.fail(this.self.MAX_OFFSET_EXCEEDED);}},onTouchEnd:function(e){if(this.onTouchMove(e)===false){return false;}
var touch=e.changedTouches[0],x=touch.pageX,y=touch.pageY,deltaX=x-this.startX,deltaY=y-this.startY,absDeltaX=Math.abs(deltaX),absDeltaY=Math.abs(deltaY),minDistance=this.getMinDistance(),duration=e.time-this.startTime,direction,distance;if(this.isVertical&&absDeltaY<minDistance){this.isVertical=false;}
if(this.isHorizontal&&absDeltaX<minDistance){this.isHorizontal=false;}
if(this.isHorizontal){direction=(deltaX<0)?'left':'right';distance=absDeltaX;}
else if(this.isVertical){direction=(deltaY<0)?'up':'down';distance=absDeltaY;}
else{return this.fail(this.self.DISTANCE_NOT_ENOUGH);}
this.fire('swipe',e,[touch],{touch:touch,direction:direction,distance:distance,duration:duration});}});Ext.define('Ext.event.recognizer.HorizontalSwipe',{extend:'Ext.event.recognizer.Swipe',handledEvents:['swipe'],onTouchStart:function(e){if(this.callParent(arguments)===false){return false;}
var touch=e.changedTouches[0];this.startTime=e.time;this.startX=touch.pageX;this.startY=touch.pageY;},onTouchMove:function(e){var touch=e.changedTouches[0],y=touch.pageY,absDeltaY=Math.abs(y-this.startY),time=e.time,maxDuration=this.getMaxDuration(),maxOffset=this.getMaxOffset();if(time-this.startTime>maxDuration){return this.fail(this.self.MAX_DURATION_EXCEEDED);}
if(absDeltaY>maxOffset){return this.fail(this.self.MAX_OFFSET_EXCEEDED);}},onTouchEnd:function(e){if(this.onTouchMove(e)!==false){var touch=e.changedTouches[0],x=touch.pageX,deltaX=x-this.startX,distance=Math.abs(deltaX),duration=e.time-this.startTime,minDistance=this.getMinDistance(),direction;if(distance<minDistance){return this.fail(this.self.DISTANCE_NOT_ENOUGH);}
direction=(deltaX<0)?'left':'right';this.fire('swipe',e,[touch],{touch:touch,direction:direction,distance:distance,duration:duration});}}});Ext.define('Ext.event.recognizer.MultiTouch',{extend:'Ext.event.recognizer.Touch',requiredTouchesCount:2,isTracking:false,isStarted:false,onTouchStart:function(e){var requiredTouchesCount=this.requiredTouchesCount,touches=e.touches,touchesCount=touches.length;if(touchesCount===requiredTouchesCount){this.start(e);}
else if(touchesCount>requiredTouchesCount){this.end(e);}},onTouchEnd:function(e){this.end(e);},start:function(){if(!this.isTracking){this.isTracking=true;this.isStarted=false;}},end:function(e){if(this.isTracking){this.isTracking=false;if(this.isStarted){this.isStarted=false;this.fireEnd(e);}}}});Ext.define('Ext.event.recognizer.Pinch',{extend:'Ext.event.recognizer.MultiTouch',requiredTouchesCount:2,handledEvents:['pinchstart','pinch','pinchend'],startDistance:0,lastTouches:null,onTouchMove:function(e){if(!this.isTracking){return;}
var touches=Array.prototype.slice.call(e.touches),firstPoint,secondPoint,distance;firstPoint=touches[0].point;secondPoint=touches[1].point;distance=firstPoint.getDistanceTo(secondPoint);if(distance===0){return;}
if(!this.isStarted){this.isStarted=true;this.startDistance=distance;this.fire('pinchstart',e,touches,{touches:touches,distance:distance,scale:1});}
else{this.fire('pinch',e,touches,{touches:touches,distance:distance,scale:distance/this.startDistance});}
this.lastTouches=touches;},fireEnd:function(e){this.fire('pinchend',e,this.lastTouches);},fail:function(){return this.callParent(arguments);}});Ext.define('Ext.event.recognizer.Rotate',{extend:'Ext.event.recognizer.MultiTouch',requiredTouchesCount:2,handledEvents:['rotatestart','rotate','rotateend'],startAngle:0,lastTouches:null,lastAngle:null,onTouchMove:function(e){if(!this.isTracking){return;}
var touches=Array.prototype.slice.call(e.touches),lastAngle=this.lastAngle,firstPoint,secondPoint,angle,nextAngle,previousAngle,diff;firstPoint=touches[0].point;secondPoint=touches[1].point;angle=firstPoint.getAngleTo(secondPoint);if(lastAngle!==null){diff=Math.abs(lastAngle-angle);nextAngle=angle+360;previousAngle=angle-360;if(Math.abs(nextAngle-lastAngle)<diff){angle=nextAngle;}
else if(Math.abs(previousAngle-lastAngle)<diff){angle=previousAngle;}}
this.lastAngle=angle;if(!this.isStarted){this.isStarted=true;this.startAngle=angle;this.fire('rotatestart',e,touches,{touches:touches,angle:angle,rotation:0});}
else{this.fire('rotate',e,touches,{touches:touches,angle:angle,rotation:angle-this.startAngle});}
this.lastTouches=touches;},fireEnd:function(e){this.lastAngle=null;this.fire('rotateend',e,this.lastTouches);}});Ext.define('Ext.ComponentQuery',{singleton:true,uses:['Ext.ComponentManager']},function(){var cq=this,filterFnPattern=['var r = [],','i = 0,','it = items,','l = it.length,','c;','for (; i < l; i++) {','c = it[i];','if (c.{0}) {','r.push(c);','}','}','return r;'].join(''),filterItems=function(items,operation){return operation.method.apply(this,[items].concat(operation.args));},getItems=function(items,mode){var result=[],i=0,length=items.length,candidate,deep=mode!=='>';for(;i<length;i++){candidate=items[i];if(candidate.getRefItems){result=result.concat(candidate.getRefItems(deep));}}
return result;},getAncestors=function(items){var result=[],i=0,length=items.length,candidate;for(;i<length;i++){candidate=items[i];while(!!(candidate=(candidate.ownerCt||candidate.floatParent))){result.push(candidate);}}
return result;},filterByXType=function(items,xtype,shallow){if(xtype==='*'){return items.slice();}
else{var result=[],i=0,length=items.length,candidate;for(;i<length;i++){candidate=items[i];if(candidate.isXType(xtype,shallow)){result.push(candidate);}}
return result;}},filterByClassName=function(items,className){var EA=Ext.Array,result=[],i=0,length=items.length,candidate;for(;i<length;i++){candidate=items[i];if(candidate.el?candidate.el.hasCls(className):EA.contains(candidate.initCls(),className)){result.push(candidate);}}
return result;},filterByAttribute=function(items,property,operator,value){var result=[],i=0,length=items.length,candidate,getter,getValue;for(;i<length;i++){candidate=items[i];getter=Ext.Class.getConfigNameMap(property).get;if(candidate[getter]){getValue=candidate[getter]();if(!value?!!getValue:(String(getValue)===value)){result.push(candidate);}}
else if(candidate.config&&candidate.config[property]){if(!value?!!candidate.config[property]:(String(candidate.config[property])===value)){result.push(candidate);}}
else if(!value?!!candidate[property]:(String(candidate[property])===value)){result.push(candidate);}}
return result;},filterById=function(items,id){var result=[],i=0,length=items.length,candidate;for(;i<length;i++){candidate=items[i];if(candidate.getId()===id||candidate.getItemId()===id){result.push(candidate);}}
return result;},filterByPseudo=function(items,name,value){return cq.pseudos[name](items,value);},modeRe=/^(\s?([>\^])\s?|\s|$)/,tokenRe=/^(#)?([\w\-]+|\*)(?:\((true|false)\))?/,matchers=[{re:/^\.([\w\-]+)(?:\((true|false)\))?/,method:filterByXType},{re:/^(?:[\[](?:@)?([\w\-]+)\s?(?:(=|.=)\s?['"]?(.*?)["']?)?[\]])/,method:filterByAttribute},{re:/^#([\w\-]+)/,method:filterById},{re:/^\:([\w\-]+)(?:\(((?:\{[^\}]+\})|(?:(?!\{)[^\s>\/]*?(?!\})))\))?/,method:filterByPseudo},{re:/^(?:\{([^\}]+)\})/,method:filterFnPattern}];cq.Query=Ext.extend(Object,{constructor:function(cfg){cfg=cfg||{};Ext.apply(this,cfg);},execute:function(root){var operations=this.operations,i=0,length=operations.length,operation,workingItems;if(!root){workingItems=Ext.ComponentManager.all.getArray();}
else if(Ext.isArray(root)){workingItems=root;}
for(;i<length;i++){operation=operations[i];if(operation.mode==='^'){workingItems=getAncestors(workingItems||[root]);}
else if(operation.mode){workingItems=getItems(workingItems||[root],operation.mode);}
else{workingItems=filterItems(workingItems||getItems([root]),operation);}
if(i===length-1){return workingItems;}}
return[];},is:function(component){var operations=this.operations,components=Ext.isArray(component)?component:[component],originalLength=components.length,lastOperation=operations[operations.length-1],ln,i;components=filterItems(components,lastOperation);if(components.length===originalLength){if(operations.length>1){for(i=0,ln=components.length;i<ln;i++){if(Ext.Array.indexOf(this.execute(),components[i])===-1){return false;}}}
return true;}
return false;}});Ext.apply(this,{cache:{},pseudos:{not:function(components,selector){var CQ=Ext.ComponentQuery,i=0,length=components.length,results=[],index=-1,component;for(;i<length;++i){component=components[i];if(!CQ.is(component,selector)){results[++index]=component;}}
return results;}},query:function(selector,root){var selectors=selector.split(','),length=selectors.length,i=0,results=[],noDupResults=[],dupMatcher={},query,resultsLn,cmp;for(;i<length;i++){selector=Ext.String.trim(selectors[i]);query=this.parse(selector);results=results.concat(query.execute(root));}
if(length>1){resultsLn=results.length;for(i=0;i<resultsLn;i++){cmp=results[i];if(!dupMatcher[cmp.id]){noDupResults.push(cmp);dupMatcher[cmp.id]=true;}}
results=noDupResults;}
return results;},is:function(component,selector){if(!selector){return true;}
var query=this.cache[selector];if(!query){this.cache[selector]=query=this.parse(selector);}
return query.is(component);},parse:function(selector){var operations=[],length=matchers.length,lastSelector,tokenMatch,matchedChar,modeMatch,selectorMatch,i,matcher,method;while(selector&&lastSelector!==selector){lastSelector=selector;tokenMatch=selector.match(tokenRe);if(tokenMatch){matchedChar=tokenMatch[1];if(matchedChar==='#'){operations.push({method:filterById,args:[Ext.String.trim(tokenMatch[2])]});}
else if(matchedChar==='.'){operations.push({method:filterByClassName,args:[Ext.String.trim(tokenMatch[2])]});}
else{operations.push({method:filterByXType,args:[Ext.String.trim(tokenMatch[2]),Boolean(tokenMatch[3])]});}
selector=selector.replace(tokenMatch[0],'');}
while(!(modeMatch=selector.match(modeRe))){for(i=0;selector&&i<length;i++){matcher=matchers[i];selectorMatch=selector.match(matcher.re);method=matcher.method;if(selectorMatch){operations.push({method:Ext.isString(matcher.method)?Ext.functionFactory('items',Ext.String.format.apply(Ext.String,[method].concat(selectorMatch.slice(1)))):matcher.method,args:selectorMatch.slice(1)});selector=selector.replace(selectorMatch[0],'');break;}
if(i===(length-1)){Ext.Error.raise('Invalid ComponentQuery selector: "'+arguments[0]+'"');}}}
if(modeMatch[1]){operations.push({mode:modeMatch[2]||modeMatch[1]});selector=selector.replace(modeMatch[0],'');}}
return new cq.Query({operations:operations});}});});Ext.define('Ext.event.publisher.ComponentDelegation',{extend:'Ext.event.publisher.Publisher',requires:['Ext.Component','Ext.ComponentQuery'],targetType:'component',optimizedSelectorRegex:/^#([\w\-]+)((?:[\s]*)>(?:[\s]*)|(?:\s*))([\w\-]+)$/i,handledEvents:['*'],getSubscribers:function(eventName,createIfNotExist){var subscribers=this.subscribers,eventSubscribers=subscribers[eventName];if(!eventSubscribers&&createIfNotExist){eventSubscribers=subscribers[eventName]={type:{$length:0},selector:[],$length:0}}
return eventSubscribers;},subscribe:function(target,eventName){if(this.idSelectorRegex.test(target)){return false;}
var optimizedSelector=target.match(this.optimizedSelectorRegex),subscribers=this.getSubscribers(eventName,true),typeSubscribers=subscribers.type,selectorSubscribers=subscribers.selector,id,isDescendant,type,map,subMap;if(optimizedSelector!==null){id=optimizedSelector[1];isDescendant=optimizedSelector[2].indexOf('>')===-1;type=optimizedSelector[3];map=typeSubscribers[type];if(!map){typeSubscribers[type]=map={descendents:{$length:0},children:{$length:0},$length:0}}
subMap=isDescendant?map.descendents:map.children;if(subMap.hasOwnProperty(id)){subMap[id]++;return true;}
subMap[id]=1;subMap.$length++;map.$length++;typeSubscribers.$length++;}
else{if(selectorSubscribers.hasOwnProperty(target)){selectorSubscribers[target]++;return true;}
selectorSubscribers[target]=1;selectorSubscribers.push(target);}
subscribers.$length++;return true;},unsubscribe:function(target,eventName,all){var subscribers=this.getSubscribers(eventName);if(!subscribers){return false;}
var match=target.match(this.optimizedSelectorRegex),typeSubscribers=subscribers.type,selectorSubscribers=subscribers.selector,id,isDescendant,type,map,subMap;all=Boolean(all);if(match!==null){id=match[1];isDescendant=match[2].indexOf('>')===-1;type=match[3];map=typeSubscribers[type];if(!map){return true;}
subMap=isDescendant?map.descendents:map.children;if(!subMap.hasOwnProperty(id)||(!all&&--subMap[id]>0)){return true;}
delete subMap[id];subMap.$length--;map.$length--;typeSubscribers.$length--;}
else{if(!selectorSubscribers.hasOwnProperty(target)||(!all&&--selectorSubscribers[target]>0)){return true;}
delete selectorSubscribers[target];Ext.Array.remove(selectorSubscribers,target);}
if(--subscribers.$length===0){delete this.subscribers[eventName];}
return true;},notify:function(target,eventName){var subscribers=this.getSubscribers(eventName),id,component;if(!subscribers||subscribers.$length===0){return false;}
id=target.substr(1);component=Ext.ComponentManager.get(id);if(component){this.dispatcher.doAddListener(this.targetType,target,eventName,'publish',this,{args:[eventName,component]},'before');}},matchesSelector:function(component,selector){return Ext.ComponentQuery.is(component,selector);},dispatch:function(target,eventName,args,connectedController){this.dispatcher.doDispatchEvent(this.targetType,target,eventName,args,null,connectedController);},publish:function(eventName,component){var subscribers=this.getSubscribers(eventName);if(!subscribers){return;}
var eventController=arguments[arguments.length-1],typeSubscribers=subscribers.type,selectorSubscribers=subscribers.selector,args=Array.prototype.slice.call(arguments,2,-2),types=component.xtypesChain,descendentsSubscribers,childrenSubscribers,parentId,ancestorIds,ancestorId,parentComponent,selector,i,ln,type,j,subLn;for(i=0,ln=types.length;i<ln;i++){type=types[i];subscribers=typeSubscribers[type];if(subscribers&&subscribers.$length>0){descendentsSubscribers=subscribers.descendents;if(descendentsSubscribers.$length>0){if(!ancestorIds){ancestorIds=component.getAncestorIds();}
for(j=0,subLn=ancestorIds.length;j<subLn;j++){ancestorId=ancestorIds[j];if(descendentsSubscribers.hasOwnProperty(ancestorId)){this.dispatch('#'+ancestorId+' '+type,eventName,args,eventController);}}}
childrenSubscribers=subscribers.children;if(childrenSubscribers.$length>0){if(!parentId){if(ancestorIds){parentId=ancestorIds[0];}
else{parentComponent=component.getParent();if(parentComponent){parentId=parentComponent.getId();}}}
if(parentId){if(childrenSubscribers.hasOwnProperty(parentId)){this.dispatch('#'+parentId+' > '+type,eventName,args,eventController);}}}}}
ln=selectorSubscribers.length;if(ln>0){for(i=0;i<ln;i++){selector=selectorSubscribers[i];if(this.matchesSelector(component,selector)){this.dispatch(selector,eventName,args,eventController);}}}}});Ext.define('Ext.event.publisher.ComponentPaint',{extend:'Ext.event.publisher.Publisher',targetType:'component',handledEvents:['painted','erased'],eventNames:{painted:'painted',erased:'erased'},constructor:function(){this.callParent(arguments);this.hiddenQueue={};this.renderedQueue={};},getSubscribers:function(eventName,createIfNotExist){var subscribers=this.subscribers;if(!subscribers.hasOwnProperty(eventName)){if(!createIfNotExist){return null;}
subscribers[eventName]={$length:0};}
return subscribers[eventName];},setDispatcher:function(dispatcher){var targetType=this.targetType;dispatcher.doAddListener(targetType,'*','renderedchange','onBeforeComponentRenderedChange',this,null,'before');dispatcher.doAddListener(targetType,'*','hiddenchange','onBeforeComponentHiddenChange',this,null,'before');dispatcher.doAddListener(targetType,'*','renderedchange','onComponentRenderedChange',this,null,'after');dispatcher.doAddListener(targetType,'*','hiddenchange','onComponentHiddenChange',this,null,'after');return this.callParent(arguments);},subscribe:function(target,eventName){var match=target.match(this.idSelectorRegex),subscribers,id;if(!match){return false;}
id=match[1];subscribers=this.getSubscribers(eventName,true);if(subscribers.hasOwnProperty(id)){subscribers[id]++;return true;}
subscribers[id]=1;subscribers.$length++;return true;},unsubscribe:function(target,eventName,all){var match=target.match(this.idSelectorRegex),subscribers,id;if(!match||!(subscribers=this.getSubscribers(eventName))){return false;}
id=match[1];if(!subscribers.hasOwnProperty(id)||(!all&&--subscribers[id]>0)){return true;}
delete subscribers[id];if(--subscribers.$length===0){delete this.subscribers[eventName];}
return true;},onBeforeComponentRenderedChange:function(container,component,rendered){var eventNames=this.eventNames,eventName=rendered?eventNames.painted:eventNames.erased,subscribers=this.getSubscribers(eventName),queue;if(subscribers&&subscribers.$length>0){this.renderedQueue[component.getId()]=queue=[];this.publish(subscribers,component,eventName,queue);}},onBeforeComponentHiddenChange:function(component,hidden){var eventNames=this.eventNames,eventName=hidden?eventNames.erased:eventNames.painted,subscribers=this.getSubscribers(eventName),queue;if(subscribers&&subscribers.$length>0){this.hiddenQueue[component.getId()]=queue=[];this.publish(subscribers,component,eventName,queue);}},onComponentRenderedChange:function(container,component){var renderedQueue=this.renderedQueue,id=component.getId(),queue;if(!renderedQueue.hasOwnProperty(id)){return;}
queue=renderedQueue[id];delete renderedQueue[id];if(queue.length>0){this.dispatchQueue(queue);}},onComponentHiddenChange:function(component){var hiddenQueue=this.hiddenQueue,id=component.getId(),queue;if(!hiddenQueue.hasOwnProperty(id)){return;}
queue=hiddenQueue[id];delete hiddenQueue[id];if(queue.length>0){this.dispatchQueue(queue);}},dispatchQueue:function(dispatchingQueue){var dispatcher=this.dispatcher,targetType=this.targetType,eventNames=this.eventNames,queue=dispatchingQueue.slice(),ln=queue.length,i,item,component,eventName,isPainted;dispatchingQueue.length=0;if(ln>0){for(i=0;i<ln;i++){item=queue[i];component=item.component;eventName=item.eventName;isPainted=component.isPainted();if((eventName===eventNames.painted&&isPainted)||eventName===eventNames.erased&&!isPainted){dispatcher.doDispatchEvent(targetType,'#'+item.id,eventName,[component]);}}
queue.length=0;}},publish:function(subscribers,component,eventName,dispatchingQueue){var id=component.getId(),needsDispatching=false,eventNames,items,i,ln,isPainted;if(subscribers[id]){eventNames=this.eventNames;isPainted=component.isPainted();if((eventName===eventNames.painted&&!isPainted)||eventName===eventNames.erased&&isPainted){needsDispatching=true;}
else{return this;}}
if(component.isContainer){items=component.getItems().items;for(i=0,ln=items.length;i<ln;i++){this.publish(subscribers,items[i],eventName,dispatchingQueue);}}
else if(component.isDecorator){this.publish(subscribers,component.getComponent(),eventName,dispatchingQueue);}
if(needsDispatching){dispatchingQueue.push({id:id,eventName:eventName,component:component});}}});Ext.define('Ext.event.publisher.ComponentSize',{extend:'Ext.event.publisher.Publisher',requires:['Ext.ComponentManager','Ext.util.SizeMonitor'],targetType:'component',handledEvents:['resize'],constructor:function(){this.callParent(arguments);this.sizeMonitors={};},subscribe:function(target){var match=target.match(this.idSelectorRegex),subscribers=this.subscribers,sizeMonitors=this.sizeMonitors,dispatcher=this.dispatcher,targetType=this.targetType,component;if(!match){return false;}
if(!subscribers.hasOwnProperty(target)){subscribers[target]=0;dispatcher.addListener(targetType,target,'painted','onComponentPainted',this,null,'before');component=Ext.ComponentManager.get(match[1]);if(!component){Ext.Logger.error("Adding a listener to the 'resize' event of a non-existing component");}
sizeMonitors[target]=new Ext.util.SizeMonitor({element:component.element,callback:this.onComponentSizeChange,scope:this,args:[this,target]});}
subscribers[target]++;return true;},unsubscribe:function(target,eventName,all){var match=target.match(this.idSelectorRegex),subscribers=this.subscribers,dispatcher=this.dispatcher,targetType=this.targetType,sizeMonitors=this.sizeMonitors;if(!match){return false;}
if(!subscribers.hasOwnProperty(target)||(!all&&--subscribers[target]>0)){return true;}
sizeMonitors[target].destroy();delete sizeMonitors[target];dispatcher.removeListener(targetType,target,'painted','onComponentPainted',this,'before');delete subscribers[target];return true;},onComponentPainted:function(component){var observableId=component.getObservableId(),sizeMonitor=this.sizeMonitors[observableId];sizeMonitor.refresh();},onComponentSizeChange:function(component,observableId){this.dispatcher.doDispatchEvent(this.targetType,observableId,'resize',[component]);}});Ext.define('Ext.log.Base',{config:{},constructor:function(config){this.initConfig(config);return this;}});(function(){var Logger=Ext.define('Ext.log.Logger',{extend:'Ext.log.Base',statics:{defaultPriority:'info',priorities:{verbose:0,info:1,deprecate:2,warn:3,error:4}},config:{enabled:true,minPriority:'deprecate',writers:{}},log:function(message,priority,callerId){if(!this.getEnabled()){return this;}
var statics=Logger,priorities=statics.priorities,priorityValue=priorities[priority],caller=this.log.caller,callerDisplayName='',writers=this.getWriters(),event,i,originalCaller;if(!priority){priority='info';}
if(priorities[this.getMinPriority()]>priorityValue){return this;}
if(!callerId){callerId=1;}
if(Ext.isArray(message)){message=message.join(" ");}
else{message=String(message);}
if(typeof callerId=='number'){i=callerId;do{i--;caller=caller.caller;if(!caller){break;}
if(!originalCaller){originalCaller=caller.caller;}
if(i<=0&&caller.displayName){break;}}
while(caller!==originalCaller);callerDisplayName=Ext.getDisplayName(caller);}
else{caller=caller.caller;callerDisplayName=Ext.getDisplayName(callerId)+'#'+caller.$name;}
event={time:Ext.Date.now(),priority:priorityValue,priorityName:priority,message:message,caller:caller,callerDisplayName:callerDisplayName};for(i in writers){if(writers.hasOwnProperty(i)){writers[i].write(Ext.merge({},event));}}
return this;}},function(){Ext.Object.each(this.priorities,function(priority){this.override(priority,function(message,callerId){if(!callerId){callerId=1;}
if(typeof callerId=='number'){callerId+=1;}
this.log(message,priority,callerId);});},this);});})();Ext.define('Ext.log.formatter.Formatter',{extend:'Ext.log.Base',config:{messageFormat:"{message}"},format:function(event){return this.substitute(this.getMessageFormat(),event);},substitute:function(template,data){var name,value;for(name in data){if(data.hasOwnProperty(name)){value=data[name];template=template.replace(new RegExp("\\{"+name+"\\}","g"),value);}}
return template;}});Ext.define('Ext.log.writer.Writer',{extend:'Ext.log.Base',requires:['Ext.log.formatter.Formatter'],config:{formatter:null,filters:{}},constructor:function(){this.activeFilters=[];return this.callParent(arguments);},updateFilters:function(filters){var activeFilters=this.activeFilters,i,filter;activeFilters.length=0;for(i in filters){if(filters.hasOwnProperty(i)){filter=filters[i];activeFilters.push(filter);}}},write:function(event){var filters=this.activeFilters,formatter=this.getFormatter(),i,ln,filter;for(i=0,ln=filters.length;i<ln;i++){filter=filters[i];if(!filters[i].accept(event)){return this;}}
if(formatter){event.message=formatter.format(event);}
this.doWrite(event);return this;},doWrite:Ext.emptyFn});Ext.define('Ext.log.writer.Console',{extend:'Ext.log.writer.Writer',config:{throwOnErrors:true,throwOnWarnings:false},doWrite:function(event){var message=event.message,priority=event.priorityName,consoleMethod;if(priority==='error'&&this.getThrowOnErrors()){throw new Error(message);}
if(typeof console!=='undefined'){consoleMethod=priority;if(consoleMethod==='deprecate'){consoleMethod='warn';}
if(consoleMethod==='warn'&&this.getThrowOnWarnings()){throw new Error(message);}
if(!(consoleMethod in console)){consoleMethod='log';}
console[consoleMethod](message);}}});Ext.define('Ext.log.formatter.Default',{extend:'Ext.log.formatter.Formatter',config:{messageFormat:"[{priorityName}][{callerDisplayName}] {message}"},format:function(event){var event=Ext.merge({},event,{priorityName:event.priorityName.toUpperCase()});return this.callParent([event]);}});Ext.define('Ext.fx.runner.Css',{extend:'Ext.Evented',requires:['Ext.fx.Animation'],prefixedProperties:{'transform':true,'transform-origin':true,'perspective':true,'transform-style':true,'transition':true,'transition-property':true,'transition-duration':true,'transition-timing-function':true,'transition-delay':true,'animation':true,'animation-name':true,'animation-duration':true,'animation-iteration-count':true,'animation-direction':true,'animation-timing-function':true,'animation-delay':true},lengthProperties:{'top':true,'right':true,'bottom':true,'left':true,'width':true,'height':true,'max-height':true,'max-width':true,'min-height':true,'min-width':true,'margin-bottom':true,'margin-left':true,'margin-right':true,'margin-top':true,'padding-bottom':true,'padding-left':true,'padding-right':true,'padding-top':true,'border-bottom-width':true,'border-left-width':true,'border-right-width':true,'border-spacing':true,'border-top-width':true,'border-width':true,'outline-width':true,'letter-spacing':true,'line-height':true,'text-indent':true,'word-spacing':true,'font-size':true,'translate':true,'translateX':true,'translateY':true,'translateZ':true,'translate3d':true},durationProperties:{'transition-duration':true,'transition-delay':true,'animation-duration':true,'animation-delay':true},angleProperties:{rotate:true,rotateX:true,rotateY:true,rotateZ:true,skew:true,skewX:true,skewY:true},lengthUnitRegex:/([a-z%]*)$/,DEFAULT_UNIT_LENGTH:'px',DEFAULT_UNIT_ANGLE:'deg',DEFAULT_UNIT_DURATION:'ms',formattedNameCache:{},constructor:function(){var supports3dTransform=Ext.feature.has.Css3dTransforms;if(supports3dTransform){this.transformMethods=['translateX','translateY','translateZ','rotate','rotateX','rotateY','rotateZ','skewX','skewY','scaleX','scaleY','scaleZ'];}
else{this.transformMethods=['translateX','translateY','rotate','skewX','skewY','scaleX','scaleY'];}
this.vendorPrefix=Ext.browser.getStyleDashPrefix();this.ruleStylesCache={};return this;},getStyleSheet:function(){var styleSheet=this.styleSheet,styleElement,styleSheets;if(!styleSheet){styleElement=document.createElement('style');styleElement.type='text/css';(document.head||document.getElementsByTagName('head')[0]).appendChild(styleElement);styleSheets=document.styleSheets;this.styleSheet=styleSheet=styleSheets[styleSheets.length-1];}
return styleSheet;},applyRules:function(selectors){var styleSheet=this.getStyleSheet(),ruleStylesCache=this.ruleStylesCache,rules=styleSheet.cssRules,selector,properties,ruleStyle,ruleStyleCache,rulesLength,name,value;for(selector in selectors){properties=selectors[selector];ruleStyle=ruleStylesCache[selector];if(ruleStyle===undefined){rulesLength=rules.length;styleSheet.insertRule(selector+'{}',rulesLength);ruleStyle=ruleStylesCache[selector]=rules.item(rulesLength).style;}
ruleStyleCache=ruleStyle.$cache;if(!ruleStyleCache){ruleStyleCache=ruleStyle.$cache={};}
for(name in properties){value=this.formatValue(properties[name],name);name=this.formatName(name);if(ruleStyleCache[name]!==value){ruleStyleCache[name]=value;if(value===null){ruleStyle.removeProperty(name);}
else{ruleStyle.setProperty(name,value,'important');}}}}
return this;},applyStyles:function(styles){var id,element,elementStyle,properties,name,value;for(id in styles){element=document.getElementById(id);if(!element){return this;}
elementStyle=element.style;properties=styles[id];for(name in properties){value=this.formatValue(properties[name],name);name=this.formatName(name);if(value===null){elementStyle.removeProperty(name);}
else{elementStyle.setProperty(name,value,'important');}}}
return this;},formatName:function(name){var cache=this.formattedNameCache,formattedName=cache[name];if(!formattedName){if(this.prefixedProperties[name]){formattedName=this.vendorPrefix+name;}
else{formattedName=name;}
cache[name]=formattedName;}
return formattedName;},formatValue:function(value,name){var type=typeof value,lengthUnit=this.DEFAULT_UNIT_LENGTH,transformMethods,method,i,ln,transformValues,values,unit;if(type=='string'){if(this.lengthProperties[name]){unit=value.match(this.lengthUnitRegex)[1];if(unit.length>0){if(unit!==lengthUnit){Ext.Logger.error("Length unit: '"+unit+"' in value: '"+value+"' of property: '"+name+"' is not "+"valid for animation. Only 'px' is allowed");}}
else{return value+lengthUnit;}}
return value;}
else if(type=='number'){if(value==0){return'0';}
if(this.lengthProperties[name]){return value+lengthUnit;}
if(this.angleProperties[name]){return value+this.DEFAULT_UNIT_ANGLE;}
if(this.durationProperties[name]){return value+this.DEFAULT_UNIT_DURATION;}}
else if(name==='transform'){transformMethods=this.transformMethods;transformValues=[];for(i=0,ln=transformMethods.length;i<ln;i++){method=transformMethods[i];transformValues.push(method+'('+this.formatValue(value[method],method)+')');}
return transformValues.join(' ');}
else if(Ext.isArray(value)){values=[];for(i=0,ln=value.length;i<ln;i++){values.push(this.formatValue(value[i],name));}
return(values.length>0)?values.join(', '):'none';}
return value;}});Ext.define('Ext.fx.runner.CssTransition',{extend:'Ext.fx.runner.Css',listenersAttached:false,constructor:function(){this.runningAnimationsData={};return this.callParent(arguments);},attachListeners:function(){this.listenersAttached=true;this.getEventDispatcher().addListener('element','*','transitionend','onTransitionEnd',this);},onTransitionEnd:function(e){var target=e.target,id=target.id;if(id&&this.runningAnimationsData.hasOwnProperty(id)){this.refreshRunningAnimationsData(Ext.get(target),[e.browserEvent.propertyName]);}},onAnimationEnd:function(element,data,animation,isInterrupted,isReplaced){var id=element.getId(),runningData=this.runningAnimationsData[id],endRules={},endData={},runningNameMap,toPropertyNames,i,ln,name;if(runningData){runningNameMap=runningData.nameMap;}
endRules[id]=endData;if(data.onBeforeEnd){data.onBeforeEnd.call(data.scope||this,element,isInterrupted);}
animation.fireEvent('animationbeforeend',animation,element,isInterrupted);this.fireEvent('animationbeforeend',this,animation,element,isInterrupted);if(isReplaced||(!isInterrupted&&!data.preserveEndState)){toPropertyNames=data.toPropertyNames;for(i=0,ln=toPropertyNames.length;i<ln;i++){name=toPropertyNames[i];if(!runningNameMap.hasOwnProperty(name)){endData[name]=null;}}}
if(data.after){Ext.merge(endData,data.after);}
this.applyStyles(endRules);if(data.onEnd){data.onEnd.call(data.scope||this,element,isInterrupted);}
animation.fireEvent('animationend',animation,element,isInterrupted);this.fireEvent('animationend',this,animation,element,isInterrupted);},onAllAnimationsEnd:function(element){var id=element.getId(),endRules={};delete this.runningAnimationsData[id];endRules[id]={'transition-property':null,'transition-duration':null,'transition-timing-function':null,'transition-delay':null};this.applyStyles(endRules);this.fireEvent('animationallend',this,element);},hasRunningAnimations:function(element){var id=element.getId(),runningAnimationsData=this.runningAnimationsData;return runningAnimationsData.hasOwnProperty(id)&&runningAnimationsData[id].sessions.length>0;},refreshRunningAnimationsData:function(element,propertyNames,interrupt,replace){var id=element.getId(),runningAnimationsData=this.runningAnimationsData,runningData=runningAnimationsData[id],nameMap=runningData.nameMap,nameList=runningData.nameList,sessions=runningData.sessions,ln,j,subLn,name,i,session,map,list,hasCompletedSession=false;interrupt=Boolean(interrupt);replace=Boolean(replace);if(!sessions){return this;}
ln=sessions.length;if(ln===0){return this;}
if(replace){runningData.nameMap={};nameList.length=0;for(i=0;i<ln;i++){session=sessions[i];this.onAnimationEnd(element,session.data,session.animation,interrupt,replace);}
sessions.length=0;}
else{for(i=0;i<ln;i++){session=sessions[i];map=session.map;list=session.list;for(j=0,subLn=propertyNames.length;j<subLn;j++){name=propertyNames[j];if(map[name]){delete map[name];Ext.Array.remove(list,name);session.length--;if(--nameMap[name]==0){delete nameMap[name];Ext.Array.remove(nameList,name);}}}
if(session.length==0){sessions.splice(i,1);i--;ln--;hasCompletedSession=true;this.onAnimationEnd(element,session.data,session.animation,interrupt);}}}
if(!replace&&!interrupt&&sessions.length==0&&hasCompletedSession){this.onAllAnimationsEnd(element);}},getRunningData:function(id){var runningAnimationsData=this.runningAnimationsData;if(!runningAnimationsData.hasOwnProperty(id)){runningAnimationsData[id]={nameMap:{},nameList:[],sessions:[]};}
return runningAnimationsData[id];},getTestElement:function(){var testElement=this.testElement,iframe,iframeDocument,iframeStyle;if(!testElement){iframe=document.createElement('iframe');iframeStyle=iframe.style;iframeStyle.setProperty('visibility','hidden','important');iframeStyle.setProperty('width','0px','important');iframeStyle.setProperty('height','0px','important');iframeStyle.setProperty('position','absolute','important');iframeStyle.setProperty('border','0px','important');iframeStyle.setProperty('zIndex','-1000','important');document.body.appendChild(iframe);iframeDocument=iframe.contentDocument;iframeDocument.open();iframeDocument.writeln('</body>');iframeDocument.close();this.testElement=testElement=iframeDocument.createElement('div');testElement.style.setProperty('position','absolute','!important');iframeDocument.body.appendChild(testElement);this.testElementComputedStyle=window.getComputedStyle(testElement);}
return testElement;},getCssStyleValue:function(name,value){var testElement=this.getTestElement(),computedStyle=this.testElementComputedStyle,style=testElement.style;style.setProperty(name,value);value=computedStyle.getPropertyValue(name);style.removeProperty(name);return value;},run:function(animations){var me=this,isLengthPropertyMap=this.lengthProperties,fromData={},toData={},data={},element,elementId,from,to,before,fromPropertyNames,toPropertyNames,doApplyTo,message,runningData,i,j,ln,animation,propertiesLength,sessionNameMap,computedStyle,formattedName,name,toFormattedValue,computedValue,fromFormattedValue,isLengthProperty,runningNameMap,runningNameList,runningSessions;if(!this.listenersAttached){this.attachListeners();}
animations=Ext.Array.from(animations);for(i=0,ln=animations.length;i<ln;i++){animation=animations[i];animation=Ext.factory(animation,Ext.fx.Animation);element=animation.getElement();computedStyle=window.getComputedStyle(element.dom);elementId=element.getId();data=Ext.merge({},animation.getData());if(animation.onBeforeStart){animation.onBeforeStart.call(animation.scope||this,element);animation.fireEvent('animationstart',animation);this.fireEvent('animationstart',this,animation);}
data[elementId]=data;before=data.before;from=data.from;to=data.to;data.fromPropertyNames=fromPropertyNames=[];data.toPropertyNames=toPropertyNames=[];for(name in to){if(to.hasOwnProperty(name)){to[name]=toFormattedValue=this.formatValue(to[name],name);formattedName=this.formatName(name);isLengthProperty=isLengthPropertyMap.hasOwnProperty(name);if(!isLengthProperty){toFormattedValue=this.getCssStyleValue(formattedName,toFormattedValue);}
if(from.hasOwnProperty(name)){from[name]=fromFormattedValue=this.formatValue(from[name],name);if(!isLengthProperty){fromFormattedValue=this.getCssStyleValue(formattedName,fromFormattedValue);}
if(toFormattedValue!==fromFormattedValue){fromPropertyNames.push(formattedName);toPropertyNames.push(formattedName);}}
else{computedValue=computedStyle.getPropertyValue(formattedName);if(toFormattedValue!==computedValue){toPropertyNames.push(formattedName);}}}}
propertiesLength=toPropertyNames.length;if(propertiesLength===0){this.onAnimationEnd(element,data,animation);continue;}
runningData=this.getRunningData(elementId);runningSessions=runningData.sessions;if(runningSessions.length>0){this.refreshRunningAnimationsData(element,Ext.Array.merge(fromPropertyNames,toPropertyNames),true,data.replacePrevious);}
runningNameMap=runningData.nameMap;runningNameList=runningData.nameList;sessionNameMap={};for(j=0;j<propertiesLength;j++){name=toPropertyNames[j];sessionNameMap[name]=true;if(!runningNameMap.hasOwnProperty(name)){runningNameMap[name]=1;runningNameList.push(name);}
else{runningNameMap[name]++;}}
runningSessions.push({element:element,map:sessionNameMap,list:toPropertyNames.slice(),length:propertiesLength,data:data,animation:animation});fromData[elementId]=from=Ext.apply(Ext.Object.chain(before),from);if(runningNameList.length>0){fromPropertyNames=Ext.Array.difference(runningNameList,fromPropertyNames);toPropertyNames=Ext.Array.merge(fromPropertyNames,toPropertyNames);from['transition-property']=fromPropertyNames;}
toData[elementId]=to=Ext.Object.chain(to);to['transition-property']=toPropertyNames;to['transition-duration']=data.duration;to['transition-timing-function']=data.easing;to['transition-delay']=data.delay;animation.startTime=Date.now();}
message=this.$className;this.applyStyles(fromData);doApplyTo=function(e){if(e.data===message&&e.source===window){window.removeEventListener('message',doApplyTo,false);me.applyStyles(toData);}};window.addEventListener('message',doApplyTo,false);window.postMessage(message,'*');}});Ext.define('Ext.fx.Runner',{requires:['Ext.fx.runner.CssTransition'],constructor:function(){return new Ext.fx.runner.CssTransition();}});Ext.define('Ext.LoadMask',{extend:'Ext.Mask',xtype:'loadmask',config:{message:'Loading...',messageCls:Ext.baseCSSPrefix+'mask-message',indicator:true,listeners:{painted:'onPainted',erased:'onErased'}},getTemplate:function(){var prefix=Ext.baseCSSPrefix;return[{reference:'innerElement',cls:prefix+'mask-inner',children:[{reference:'indicatorElement',cls:prefix+'loading-spinner-outer',children:[{cls:prefix+'loading-spinner',children:[{tag:'span',cls:prefix+'loading-top'},{tag:'span',cls:prefix+'loading-right'},{tag:'span',cls:prefix+'loading-bottom'},{tag:'span',cls:prefix+'loading-left'}]}]},{reference:'messageElement'}]}];},updateMessage:function(newMessage){this.messageElement.setHtml(newMessage);},updateMessageCls:function(newMessageCls,oldMessageCls){this.messageElement.replaceCls(oldMessageCls,newMessageCls);},updateIndicator:function(newIndicator){this[newIndicator?'removeCls':'addCls'](Ext.baseCSSPrefix+'indicator-hidden');},onPainted:function(){this.getParent().on({scope:this,resize:this.refreshPosition});this.refreshPosition();},onErased:function(){this.getParent().un({scope:this,resize:this.refreshPosition});},refreshPosition:function(){var parent=this.getParent(),scrollable=parent.getScrollable(),scroller=(scrollable)?scrollable.getScroller():null,offset=(scroller)?scroller.position:{x:0,y:0},parentSize=parent.element.getSize(),innerSize=this.element.getSize();this.innerElement.setStyle({marginTop:Math.round(parentSize.height-innerSize.height+(offset.y*2))+'px',marginLeft:Math.round(parentSize.width-innerSize.width+offset.x)+'px'});}},function(){this.override({constructor:function(config,other){if(typeof other!=="undefined"){config=other;Ext.Logger.deprecate("You no longer need to pass an element to create a Ext.LoadMask. "+"It is a component and can be shown using the Ext.Container.masked configuration.",this);}
if(config){if(config.hasOwnProperty('msg')){config.message=config.msg;Ext.Logger.deprecate("'msg' config is deprecated, please use 'message' config instead",this);delete config.msg;}
if(config.hasOwnProperty('msgCls')){config.messageCls=config.msgCls;Ext.Logger.deprecate("'msgCls' config is deprecated, please use 'messageCls' config instead",this);delete config.msgCls;}
if(config.hasOwnProperty('store')){Ext.Logger.deprecate("'store' config has been removed. You can no longer bind a store to a Ext.LoadMask",this);delete config.store;}}
this.callParent([config]);},bindStore:function(){Ext.Logger.deprecate("You can no longer bind a store to a Ext.LoadMask",this);}});});Ext.define('Ext.viewport.Default',{extend:'Ext.Container',xtype:'viewport',PORTRAIT:'portrait',LANDSCAPE:'landscape',requires:['Ext.LoadMask'],config:{autoMaximize:false,autoBlurInput:true,preventPanning:true,preventZooming:true,autoRender:true,layout:'card',width:'100%',height:'100%'},isReady:false,isViewport:true,isMaximizing:false,id:'ext-viewport',isInputRegex:/^(input|textarea|select|a)$/i,focusedElement:null,fullscreenItemCls:Ext.baseCSSPrefix+'fullscreen',constructor:function(config){var bind=Ext.Function.bind;this.doPreventPanning=bind(this.doPreventPanning,this);this.doPreventZooming=bind(this.doPreventZooming,this);this.doBlurInput=bind(this.doBlurInput,this);this.maximizeOnEvents=['ready','orientationchange'];this.orientation=this.determineOrientation();this.windowWidth=this.getWindowWidth();this.windowHeight=this.getWindowHeight();this.windowOuterHeight=this.getWindowOuterHeight();if(!this.stretchHeights){this.stretchHeights={};}
this.callParent([config]);if(this.supportsOrientation()){this.addWindowListener('orientationchange',bind(this.onOrientationChange,this));}
else{this.addWindowListener('resize',bind(this.onResize,this));}
document.addEventListener('focus',bind(this.onElementFocus,this),true);document.addEventListener('blur',bind(this.onElementBlur,this),true);Ext.onDocumentReady(this.onDomReady,this);this.on('ready',this.onReady,this,{single:true});this.getEventDispatcher().addListener('component','*','fullscreen','onItemFullscreenChange',this);return this;},onDomReady:function(){this.isReady=true;this.updateSize();this.fireEvent('ready',this);},onReady:function(){if(this.getAutoRender()){this.render();}},onElementFocus:function(e){this.focusedElement=e.target;},onElementBlur:function(){this.focusedElement=null;},render:function(){if(!this.rendered){var body=Ext.getBody(),clsPrefix=Ext.baseCSSPrefix,classList=[],osEnv=Ext.os,osName=osEnv.name.toLowerCase(),browserName=Ext.browser.name.toLowerCase(),osMajorVersion=osEnv.version.getMajor(),orientation=this.getOrientation();this.renderTo(body);classList.push(clsPrefix+osEnv.deviceType.toLowerCase());if(osEnv.is.iPad){classList.push(clsPrefix+'ipad');}
classList.push(clsPrefix+osName);classList.push(clsPrefix+browserName);if(osMajorVersion){classList.push(clsPrefix+osName+'-'+osMajorVersion);}
if(osEnv.is.BlackBerry){classList.push(clsPrefix+'bb');}
if(Ext.browser.is.Standalone){classList.push(clsPrefix+'standalone');}
classList.push(clsPrefix+orientation);body.addCls(classList);}},applyAutoBlurInput:function(autoBlurInput){var touchstart=(Ext.feature.has.Touch)?'touchstart':'mousedown';if(autoBlurInput){this.addWindowListener(touchstart,this.doBlurInput,false);}
else{this.removeWindowListener(touchstart,this.doBlurInput,false);}
return autoBlurInput;},applyAutoMaximize:function(autoMaximize){if(autoMaximize){this.on('ready','doAutoMaximizeOnReady',this,{single:true});this.on('orientationchange','doAutoMaximizeOnOrientationChange',this);}
else{this.un('ready','doAutoMaximizeOnReady',this);this.un('orientationchange','doAutoMaximizeOnOrientationChange',this);}
return autoMaximize;},applyPreventPanning:function(preventPanning){if(preventPanning){this.addWindowListener('touchmove',this.doPreventPanning,false);}
else{this.removeWindowListener('touchmove',this.doPreventPanning,false);}
return preventPanning;},applyPreventZooming:function(preventZooming){var touchstart=(Ext.feature.has.Touch)?'touchstart':'mousedown';if(preventZooming){this.addWindowListener(touchstart,this.doPreventZooming,false);}
else{this.removeWindowListener(touchstart,this.doPreventZooming,false);}
return preventZooming;},doAutoMaximizeOnReady:function(){var controller=arguments[arguments.length-1];controller.pause();this.isMaximizing=true;this.on('maximize',function(){this.isMaximizing=false;this.updateSize();controller.resume();this.fireEvent('ready',this);},this,{single:true});this.maximize();},doAutoMaximizeOnOrientationChange:function(){var controller=arguments[arguments.length-1],firingArguments=controller.firingArguments;controller.pause();this.isMaximizing=true;this.on('maximize',function(){this.isMaximizing=false;this.updateSize();firingArguments[1]=this.windowWidth;firingArguments[2]=this.windowHeight;controller.resume();},this,{single:true});this.maximize();},doBlurInput:function(e){var target=e.target,focusedElement=this.focusedElement;if(focusedElement&&!this.isInputRegex.test(target.tagName)){delete this.focusedElement;focusedElement.blur();}},doPreventPanning:function(e){e.preventDefault();},doPreventZooming:function(e){if('button'in e&&e.button!==0){return;}
var target=e.target;if(target&&target.nodeType===1&&!this.isInputRegex.test(target.tagName)){e.preventDefault();}},addWindowListener:function(eventName,fn,capturing){window.addEventListener(eventName,fn,Boolean(capturing));},removeWindowListener:function(eventName,fn,capturing){window.removeEventListener(eventName,fn,Boolean(capturing));},doAddListener:function(eventName,fn,scope,options){if(eventName==='ready'&&this.isReady&&!this.isMaximizing){fn.call(scope);return this;}
this.mixins.observable.doAddListener.apply(this,arguments);},supportsOrientation:function(){return Ext.feature.has.Orientation;},onResize:function(){var oldWidth=this.windowWidth,oldHeight=this.windowHeight,width=this.getWindowWidth(),height=this.getWindowHeight(),currentOrientation=this.getOrientation(),newOrientation=this.determineOrientation();if((oldWidth!==width||oldHeight!==height)&&currentOrientation!==newOrientation){this.fireOrientationChangeEvent(newOrientation,currentOrientation);}},onOrientationChange:function(){var currentOrientation=this.getOrientation(),newOrientation=this.determineOrientation();if(newOrientation!==currentOrientation){this.fireOrientationChangeEvent(newOrientation,currentOrientation);}},fireOrientationChangeEvent:function(newOrientation,oldOrientation){var clsPrefix=Ext.baseCSSPrefix;Ext.getBody().replaceCls(clsPrefix+oldOrientation,clsPrefix+newOrientation);this.orientation=newOrientation;this.updateSize();this.fireEvent('orientationchange',this,newOrientation,this.windowWidth,this.windowHeight);},updateSize:function(width,height){this.windowWidth=width!==undefined?width:this.getWindowWidth();this.windowHeight=height!==undefined?height:this.getWindowHeight();return this;},waitUntil:function(condition,onSatisfied,onTimeout,delay,timeoutDuration){if(!delay){delay=50;}
if(!timeoutDuration){timeoutDuration=2000;}
var scope=this,elapse=0;setTimeout(function repeat(){elapse+=delay;if(condition.call(scope)===true){if(onSatisfied){onSatisfied.call(scope);}}
else{if(elapse>=timeoutDuration){if(onTimeout){onTimeout.call(scope);}}
else{setTimeout(repeat,delay);}}},delay);},maximize:function(){this.fireMaximizeEvent();},fireMaximizeEvent:function(){this.updateSize();this.fireEvent('maximize',this);},doSetHeight:function(height){Ext.getBody().setHeight(height);this.callParent(arguments);},doSetWidth:function(width){Ext.getBody().setWidth(width);this.callParent(arguments);},scrollToTop:function(){window.scrollTo(0,-1);},getWindowWidth:function(){return window.innerWidth;},getWindowHeight:function(){return window.innerHeight;},getWindowOuterHeight:function(){return window.outerHeight;},getWindowOrientation:function(){return window.orientation;},getOrientation:function(){return this.orientation;},getSize:function(){return{width:this.windowWidth,height:this.windowHeight};},determineOrientation:function(){var portrait=this.PORTRAIT,landscape=this.LANDSCAPE;if(this.supportsOrientation()){if(this.getWindowOrientation()%180===0){return portrait;}
return landscape;}
else{if(this.getWindowHeight()>=this.getWindowWidth()){return portrait;}
return landscape;}},onItemFullscreenChange:function(item){item.addCls(this.fullscreenItemCls);this.add(item);}});Ext.define('Ext.viewport.Ios',{extend:'Ext.viewport.Default',isFullscreen:function(){return this.isHomeScreen();},isHomeScreen:function(){return window.navigator.standalone===true;},constructor:function(){this.callParent(arguments);if(this.getAutoMaximize()&&!this.isFullscreen()){this.addWindowListener('touchstart',Ext.Function.bind(this.onTouchStart,this));}},maximize:function(){if(this.isFullscreen()){return this.callParent();}
var stretchHeights=this.stretchHeights,orientation=this.orientation,currentHeight=this.getWindowHeight(),height=stretchHeights[orientation];if(window.scrollY>0){this.scrollToTop();if(!height){stretchHeights[orientation]=height=this.getWindowHeight();}
this.setHeight(height);this.fireMaximizeEvent();}
else{if(!height){height=this.getScreenHeight();}
this.setHeight(height);this.waitUntil(function(){this.scrollToTop();return currentHeight!==this.getWindowHeight();},function(){if(!stretchHeights[orientation]){height=stretchHeights[orientation]=this.getWindowHeight();this.setHeight(height);}
this.fireMaximizeEvent();},function(){Ext.Logger.error("Timeout waiting for window.innerHeight to change",this);height=stretchHeights[orientation]=this.getWindowHeight();this.setHeight(height);this.fireMaximizeEvent();},50,1000);}},getScreenHeight:function(){return window.screen[this.orientation===this.PORTRAIT?'height':'width'];},onElementFocus:function(){if(this.getAutoMaximize()&&!this.isFullscreen()){clearTimeout(this.scrollToTopTimer);}
this.callParent(arguments);},onElementBlur:function(){if(this.getAutoMaximize()&&!this.isFullscreen()){this.scrollToTopTimer=setTimeout(this.scrollToTop,500);}
this.callParent(arguments);},onTouchStart:function(){if(this.focusedElement===null){this.scrollToTop();}},scrollToTop:function(){window.scrollTo(0,0);}},function(){if(!Ext.os.is.iOS){return;}
if(Ext.os.version.lt('3.2')){this.override({constructor:function(){var stretchHeights=this.stretchHeights={};stretchHeights[this.PORTRAIT]=416;stretchHeights[this.LANDSCAPE]=268;return this.callOverridden(arguments);}});}
if(Ext.os.version.lt('5')){this.override({fieldMaskClsTest:'-field-mask',doPreventZooming:function(e){var target=e.target;if(target&&target.nodeType===1&&!this.isInputRegex.test(target.tagName)&&target.className.indexOf(this.fieldMaskClsTest)==-1){e.preventDefault();}}});}
if(Ext.os.is.iPad){this.override({isFullscreen:function(){return true;}});}});Ext.define('Ext.viewport.Android',{extend:'Ext.viewport.Default',constructor:function(){this.on('orientationchange','doFireOrientationChangeEvent',this,{prepend:true});this.on('orientationchange','hideKeyboardIfNeeded',this,{prepend:true});return this.callParent(arguments);},getDummyInput:function(){var input=this.dummyInput,focusedElement=this.focusedElement,box=Ext.fly(focusedElement).getPageBox();if(!input){this.dummyInput=input=document.createElement('input');input.style.position='absolute';input.style.opacity='0';document.body.appendChild(input);}
input.style.left=box.left+'px';input.style.top=box.top+'px';input.style.display='';return input;},doBlurInput:function(e){var target=e.target,focusedElement=this.focusedElement,dummy;if(focusedElement&&!this.isInputRegex.test(target.tagName)){dummy=this.getDummyInput();delete this.focusedElement;dummy.focus();setTimeout(function(){dummy.style.display='none';},100);}},hideKeyboardIfNeeded:function(){var eventController=arguments[arguments.length-1],focusedElement=this.focusedElement;if(focusedElement){delete this.focusedElement;eventController.pause();if(Ext.os.version.lt('4')){focusedElement.style.display='none';}
else{focusedElement.blur();}
setTimeout(function(){focusedElement.style.display='';eventController.resume();},1000);}},doFireOrientationChangeEvent:function(){var eventController=arguments[arguments.length-1];this.orientationChanging=true;eventController.pause();this.waitUntil(function(){return this.getWindowOuterHeight()!==this.windowOuterHeight;},function(){this.windowOuterHeight=this.getWindowOuterHeight();this.updateSize();eventController.firingArguments[1]=this.windowWidth;eventController.firingArguments[2]=this.windowHeight;eventController.resume();this.orientationChanging=false;},function(){Ext.Logger.error("Timeout waiting for viewport's outerHeight to change before firing orientationchange",this);});return this;},applyAutoMaximize:function(autoMaximize){this.callParent(arguments);this.on('add','fixSize',this,{single:true});if(!autoMaximize){this.on('ready','fixSize',this,{single:true});this.onAfter('orientationchange','doFixSize',this);}
else{this.un('ready','fixSize',this);this.unAfter('orientationchange','doFixSize',this);}},fixSize:function(){this.doFixSize();},doFixSize:function(){this.setHeight(this.getWindowHeight());},getActualWindowOuterHeight:function(){return Math.round(this.getWindowOuterHeight()/window.devicePixelRatio);},maximize:function(){var stretchHeights=this.stretchHeights,orientation=this.orientation,height;height=stretchHeights[orientation];if(!height){stretchHeights[orientation]=height=this.getActualWindowOuterHeight();}
if(!this.addressBarHeight){this.addressBarHeight=height-this.getWindowHeight();}
this.setHeight(height);var isHeightMaximized=Ext.Function.bind(this.isHeightMaximized,this,[height]);this.scrollToTop();this.waitUntil(isHeightMaximized,this.fireMaximizeEvent,this.fireMaximizeEvent);},isHeightMaximized:function(height){this.scrollToTop();return this.getWindowHeight()===height;}},function(){if(!Ext.os.is.Android){return;}
var version=Ext.os.version,userAgent=Ext.browser.userAgent,isBuggy=/(htc|desire|incredible|ADR6300)/i.test(userAgent)&&version.lt('2.3');if(isBuggy){this.override({constructor:function(config){if(!config){config={};}
config.autoMaximize=false;this.watchDogTick=Ext.Function.bind(this.watchDogTick,this);setInterval(this.watchDogTick,1000);return this.callParent([config]);},watchDogTick:function(){this.watchDogLastTick=Ext.Date.now();},doPreventPanning:function(){var now=Ext.Date.now(),lastTick=this.watchDogLastTick,deltaTime=now-lastTick;if(deltaTime>=2000){return;}
return this.callParent(arguments);},doPreventZooming:function(){var now=Ext.Date.now(),lastTick=this.watchDogLastTick,deltaTime=now-lastTick;if(deltaTime>=2000){return;}
return this.callParent(arguments);}});}
if(version.match('2')){this.override({onReady:function(){this.addWindowListener('resize',Ext.Function.bind(this.onWindowResize,this));this.callParent(arguments);},scrollToTop:function(){document.body.scrollTop=100;},onWindowResize:function(){var oldWidth=this.windowWidth,oldHeight=this.windowHeight,width=this.getWindowWidth(),height=this.getWindowHeight();if(this.getAutoMaximize()&&!this.isMaximizing&&!this.orientationChanging&&window.scrollY===0&&oldWidth===width&&height<oldHeight&&((height>=oldHeight-this.addressBarHeight)||!this.focusedElement)){this.scrollToTop();}},fixSize:function(){var orientation=this.getOrientation(),outerHeight=window.outerHeight,outerWidth=window.outerWidth,actualOuterHeight;if(orientation==='landscape'&&(outerHeight<outerWidth)||orientation==='portrait'&&(outerHeight>=outerWidth)){actualOuterHeight=this.getActualWindowOuterHeight();}
else{actualOuterHeight=this.getWindowHeight();}
this.waitUntil(function(){return actualOuterHeight>this.getWindowHeight();},this.doFixSize,this.doFixSize,50,1000);}});}
else if(version.gtEq('3.1')){this.override({isHeightMaximized:function(height){this.scrollToTop();return this.getWindowHeight()===height-1;}});}
else if(version.match('3')){this.override({isHeightMaximized:function(){this.scrollToTop();return true;}})}
if(version.gtEq('4')){this.override({doBlurInput:Ext.emptyFn});}});Ext.define('Ext.viewport.Viewport',{requires:['Ext.viewport.Ios','Ext.viewport.Android'],constructor:function(config){var osName=Ext.os.name,viewportName,viewport;switch(osName){case'Android':viewportName='Android';break;case'iOS':viewportName='Ios';break;default:viewportName='Default';}
viewport=Ext.create('Ext.viewport.'+viewportName,config);return viewport;}});Ext.define('Ext.app.Controller',{mixins:{observable:"Ext.mixin.Observable"},config:{refs:{},routes:{},control:{},before:{},application:{},stores:[],models:[],views:[]},constructor:function(config){this.initConfig(config);this.mixins.observable.constructor.call(this,config);},init:Ext.emptyFn,launch:Ext.emptyFn,redirectTo:function(place){return this.getApplication().redirectTo(place);},execute:function(action,skipFilters){action.setBeforeFilters(this.getBefore()[action.getAction()]);action.execute();},applyBefore:function(before){var filters,name,length,i;for(name in before){filters=Ext.Array.from(before[name]);length=filters.length;for(i=0;i<length;i++){filters[i]=this[filters[i]];}
before[name]=filters;}
return before;},applyControl:function(config){this.control(config,this);return config;},applyRefs:function(refs){if(Ext.isArray(refs)){Ext.Logger.deprecate("In Sencha Touch 2 the refs config accepts an object but you have passed it an array.");}
this.ref(refs);return refs;},applyRoutes:function(routes){var app=this instanceof Ext.app.Application?this:this.getApplication(),router=app.getRouter(),route,url,config;for(url in routes){route=routes[url];config={controller:this.$className};if(Ext.isString(route)){config.action=route;}else{Ext.apply(config,route);}
router.connect(url,config);}
return routes;},applyStores:function(stores){return this.getFullyQualified(stores,'store');},applyModels:function(models){return this.getFullyQualified(models,'model');},applyViews:function(views){return this.getFullyQualified(views,'view');},getFullyQualified:function(items,namespace){var length=items.length,appName=this.getApplication().getName(),name,i;for(i=0;i<length;i++){name=items[i];if(Ext.isString(name)&&(Ext.Loader.getPrefix(name)===""||name===appName)){items[i]=appName+'.'+namespace+'.'+name;}}
return items;},control:function(selectors){this.getApplication().control(selectors,this);},ref:function(refs){var refName,getterName,selector,info;for(refName in refs){selector=refs[refName];getterName="get"+Ext.String.capitalize(refName);if(!this[getterName]){if(Ext.isString(refs[refName])){info={ref:refName,selector:selector};}else{info=refs[refName];}
this[getterName]=Ext.Function.pass(this.getRef,[refName,info],this);}
this.references=this.references||[];this.references.push(refName.toLowerCase());}},getRef:function(ref,info,config){this.refCache=this.refCache||{};info=info||{};config=config||{};Ext.apply(info,config);if(info.forceCreate){return Ext.ComponentManager.create(info,'component');}
var me=this,cached=me.refCache[ref];if(!cached){me.refCache[ref]=cached=Ext.ComponentQuery.query(info.selector)[0];if(!cached&&info.autoCreate){me.refCache[ref]=cached=Ext.ComponentManager.create(info,'component');}
if(cached){cached.on('destroy',function(){me.refCache[ref]=null;});}}
return cached;},hasRef:function(ref){return this.references&&this.references.indexOf(ref.toLowerCase())!==-1;},onClassExtended:function(cls,members){var prototype=this.prototype,defaultConfig=prototype.config,config=members.config||{},arrayRefs=members.refs,objectRefs={},stores=members.stores,views=members.views,format=Ext.String.format,refItem,key,length,i,functionName;for(key in defaultConfig){if(key in members&&key!="control"){if(key=="refs"){for(i=0;i<arrayRefs.length;i++){refItem=arrayRefs[i];objectRefs[refItem.ref]=refItem;}
config.refs=objectRefs;}else{config[key]=members[key];}
delete members[key];Ext.Logger.deprecate(key+' is deprecated as a property directly on the '+this.$className+' prototype. Please put it inside the config object.');}}
if(stores){length=stores.length;config.stores=stores;Ext.Logger.deprecate('\'stores\' is deprecated as a property directly on the '+this.$className+' prototype. Please move it '+'to Ext.application({ stores: ... }) instead');for(i=0;i<length;i++){functionName=format("get{0}Store",Ext.String.capitalize(stores[i]));prototype[functionName]=function(name){return function(){return Ext.StoreManager.lookup(name);};}(stores[i]);}}
if(views){length=views.length;config.views=views;Ext.Logger.deprecate('\'views\' is deprecated as a property directly on the '+this.$className+' prototype. Please move it '+'to Ext.application({ views: ... }) instead');for(i=0;i<length;i++){functionName=format("get{0}View",views[i]);prototype[functionName]=function(name){return function(){return Ext.ClassManager.classes[format("{0}.view.{1}",this.getApplication().getName(),name)];};}(views[i]);}}
members.config=config;},getModel:function(modelName){Ext.Logger.deprecate("getModel() is deprecated and considered bad practice - please just use the Model "+"name instead (e.g. MyApp.model.User vs this.getModel('User'))");var appName=this.getApplication().getName(),classes=Ext.ClassManager.classes;return classes[appName+'.model.'+modelName];},getController:function(controllerName,profile){Ext.Logger.deprecate("Ext.app.Controller#getController is deprecated and considered bad practice - "+"please use this.getApplication().getController('someController') instead");return this.getApplication().getController(controllerName,profile);}},function(){Ext.regController=function(name,config){Ext.apply(config,{extend:'Ext.app.Controller'});Ext.Logger.deprecate('[Ext.app.Controller] Ext.regController is deprecated, please use Ext.define to define a Controller as '+'with any other class. For more information see the Touch 1.x -> 2.x migration guide');Ext.define('controller.'+name,config);};});Ext.define('Ext.app.History',{mixins:['Ext.mixin.Observable'],config:{actions:[],updateUrl:true,token:''},constructor:function(config){if(Ext.feature.has.History){window.addEventListener('hashchange',Ext.bind(this.detectStateChange,this));}
else{setInterval(Ext.bind(this.detectStateChange,this),50);}
this.initConfig(config);},add:function(action,silent){this.getActions().push(Ext.factory(action,Ext.app.Action));var url=action.getUrl();if(this.getUpdateUrl()){this.setToken(url);window.location.hash=url;}
if(silent!==true){this.fireEvent('change',url);}
this.setToken(url);},back:function(){this.getActions().pop().run();},applyToken:function(token){return token[0]=='#'?token.substr(1):token;},detectStateChange:function(){var newToken=this.applyToken(window.location.hash),oldToken=this.getToken();if(newToken!=oldToken){this.onStateChange();this.setToken(newToken);}},onStateChange:function(){this.fireEvent('change',window.location.hash.substr(1));}});Ext.define('Ext.app.Profile',{mixins:{observable:"Ext.mixin.Observable"},config:{namespace:'auto',name:'auto',controllers:[],models:[],views:[],stores:[],application:null},constructor:function(config){this.initConfig(config);this.mixins.observable.constructor.apply(this,arguments);},isActive:function(){return false;},launch:Ext.emptyFn,applyNamespace:function(name){if(name=='auto'){name=this.getName();}
return name.toLowerCase();},applyName:function(name){if(name=='auto'){var pieces=this.$className.split('.');name=pieces[pieces.length-1];}
return name;},getDependencies:function(){var allClasses=[],format=Ext.String.format,appName=this.getApplication().getName(),namespace=this.getNamespace(),map={model:this.getModels(),view:this.getViews(),controller:this.getControllers(),store:this.getStores()},classType,classNames,fullyQualified;for(classType in map){classNames=[];Ext.each(map[classType],function(className){if(Ext.isString(className)){if(Ext.isString(className)&&(Ext.Loader.getPrefix(className)===""||className===appName)){className=appName+'.'+classType+'.'+namespace+'.'+className;}
classNames.push(className);allClasses.push(className);}},this);map[classType]=classNames;}
map.all=allClasses;return map;}});Ext.define('Ext.app.Action',{config:{scope:null,application:null,controller:null,action:null,args:[],url:undefined,data:{},title:null,beforeFilters:[],currentFilterIndex:-1},constructor:function(config){this.initConfig(config);this.getUrl();},execute:function(){this.resume();},resume:function(){var index=this.getCurrentFilterIndex()+1,filters=this.getBeforeFilters(),controller=this.getController(),nextFilter=filters[index];if(nextFilter){this.setCurrentFilterIndex(index);nextFilter.call(controller,this);}else{controller[this.getAction()].apply(controller,this.getArgs());}},applyUrl:function(url){if(url===null||url===undefined){url=this.urlEncode();}
return url;},applyController:function(controller){var app=this.getApplication(),profile=app.getCurrentProfile();if(Ext.isString(controller)){controller=app.getController(controller,profile?profile.getNamespace():null);}
return controller;},urlEncode:function(){var controller=this.getController(),splits;if(controller instanceof Ext.app.Controller){splits=controller.$className.split('.');controller=splits[splits.length-1];}
return controller+"/"+this.getAction();}});Ext.define('Ext.app.Route',{config:{conditions:{},url:null,controller:null,action:null,initialized:false},constructor:function(config){this.initConfig(config);},recognize:function(url){if(!this.getInitialized()){this.initialize();}
if(this.recognizes(url)){var matches=this.matchesFor(url),args=url.match(this.matcherRegex);args.shift();return Ext.applyIf(matches,{controller:this.getController(),action:this.getAction(),historyUrl:url,args:args});}},initialize:function(){this.paramMatchingRegex=new RegExp(/:([0-9A-Za-z\_]*)/g);this.paramsInMatchString=this.getUrl().match(this.paramMatchingRegex)||[];this.matcherRegex=this.createMatcherRegex(this.getUrl());this.setInitialized(true);},recognizes:function(url){return this.matcherRegex.test(url);},matchesFor:function(url){var params={},keys=this.paramsInMatchString,values=url.match(this.matcherRegex),length=keys.length,i;values.shift();for(i=0;i<length;i++){params[keys[i].replace(":","")]=values[i];}
return params;},argsFor:function(url){var args=[],keys=this.paramsInMatchString,values=url.match(this.matcherRegex),length=keys.length,i;values.shift();for(i=0;i<length;i++){args.push(keys[i].replace(':',""));params[keys[i].replace(":","")]=values[i];}
return params;},urlFor:function(config){var url=this.getUrl();for(var key in config){url=url.replace(":"+key,config[key]);}
return url;},createMatcherRegex:function(url){var paramsInMatchString=this.paramsInMatchString,length=paramsInMatchString.length,i,cond,matcher;for(i=0;i<length;i++){cond=this.getConditions()[paramsInMatchString[i]];matcher=Ext.util.Format.format("({0})",cond||"[%a-zA-Z0-9\-\\_\\s,]+");url=url.replace(new RegExp(paramsInMatchString[i]),matcher);}
return new RegExp("^"+url+"$");}});Ext.define('Ext.app.Router',{requires:['Ext.app.Route'],config:{routes:[],defaults:{action:'index'}},constructor:function(config){this.initConfig(config);},connect:function(url,params){params=Ext.apply({url:url},params||{},this.getDefaults());var route=Ext.create('Ext.app.Route',params);this.getRoutes().push(route);return route;},recognize:function(url){var routes=this.getRoutes(),length=routes.length,i,result;for(i=0;i<length;i++){result=routes[i].recognize(url);if(result!==undefined){return result;}}
return undefined;},draw:function(fn){fn.call(this,this);},clear:function(){this.setRoutes([]);}},function(){Ext.Router={};var drawStack=[];Ext.Router.setAppInstance=function(app){Ext.Router.appInstance=app;if(drawStack.length>0){Ext.each(drawStack,Ext.Router.draw);}};Ext.Router.draw=function(mapperFn){Ext.Logger.deprecate('Ext.Router.map is deprecated, please define your routes inline inside each Controller. '+'Please see the 1.x -> 2.x migration guide for more details.');var app=Ext.Router.appInstance,router;if(app){router=app.getRouter();mapperFn(router);}else{drawStack.push(mapperFn);}};});Ext.define('Ext.app.Application',{extend:'Ext.app.Controller',requires:['Ext.app.History','Ext.app.Profile','Ext.app.Router','Ext.app.Action'],config:{profiles:[],controllers:[],history:{},name:null,appFolder:'app',router:{},controllerInstances:[],profileInstances:[],currentProfile:null,launch:Ext.emptyFn,enableLoader:true,requires:[]},constructor:function(config){config=config||{};Ext.applyIf(config,{application:this});this.initConfig(config);for(var key in config){this[key]=config[key];}
if(config.autoCreateViewport){Ext.Logger.deprecate('[Ext.app.Application] autoCreateViewport has been deprecated in Sencha Touch 2. Please implement a '+'launch function on your Application instead and use Ext.create("MyApp.view.Main") to create your initial UI.');}
Ext.Loader.setConfig({enabled:true});Ext.require(this.getRequires(),function(){if(this.getEnableLoader()!==false){Ext.require(this.getProfiles(),this.onProfilesLoaded,this);}},this);},dispatch:function(action,addToHistory){action=action||{};Ext.applyIf(action,{application:this});action=Ext.factory(action,Ext.app.Action);if(action){var profile=this.getCurrentProfile(),profileNS=profile?profile.getNamespace():undefined,controller=this.getController(action.getController(),profileNS);if(controller){if(addToHistory!==false){this.getHistory().add(action,true);}
controller.execute(action);}}},redirectTo:function(url){if(Ext.data&&Ext.data.Model&&url instanceof Ext.data.Model){var record=url;url=record.toUrl();}
var decoded=this.getRouter().recognize(url);if(decoded){decoded.url=url;if(record){decoded.data={};decoded.data.record=record;}
return this.dispatch(decoded);}},control:function(selectors,controller){controller=controller||this;var dispatcher=this.getEventDispatcher(),refs=(controller)?controller.getRefs():{},selector,eventName,listener,listeners,ref;for(selector in selectors){if(selectors.hasOwnProperty(selector)){listeners=selectors[selector];ref=refs[selector];if(ref){selector=ref.selector||ref;}
for(eventName in listeners){listener=listeners[eventName];if(Ext.isString(listener)){listener=controller[listener];}
dispatcher.addListener('component',selector,eventName,listener,controller);}}}},getController:function(name,profileName){var instances=this.getControllerInstances(),appName=this.getName(),format=Ext.String.format,topLevelName;if(name instanceof Ext.app.Controller){return name;}
if(instances[name]){return instances[name];}else{topLevelName=format("{0}.controller.{1}",appName,name);profileName=format("{0}.controller.{1}.{2}",appName,profileName,name);return instances[profileName]||instances[topLevelName];}},onProfilesLoaded:function(){var profiles=this.getProfiles(),length=profiles.length,instances=[],requires=this.gatherDependencies(),current,i,profileDeps;for(i=0;i<length;i++){instances[i]=Ext.create(profiles[i],{application:this});profileDeps=instances[i].getDependencies();requires=requires.concat(profileDeps.all);if(instances[i].isActive()&&!current){current=instances[i];this.setCurrentProfile(current);this.setControllers(this.getControllers().concat(profileDeps.controller));this.setModels(this.getModels().concat(profileDeps.model));this.setViews(this.getViews().concat(profileDeps.view));this.setStores(this.getStores().concat(profileDeps.store));}}
this.setProfileInstances(instances);Ext.require(requires,this.loadControllerDependencies,this);},loadControllerDependencies:function(){this.instantiateControllers();var controllers=this.getControllerInstances(),classes=[],stores=[],i,controller,controllerStores,name;for(name in controllers){controller=controllers[name];controllerStores=controller.getStores();stores=stores.concat(controllerStores);classes=classes.concat(controller.getModels().concat(controller.getViews()).concat(controllerStores));}
this.setStores(this.getStores().concat(stores));Ext.require(classes,this.onDependenciesLoaded,this);},onDependenciesLoaded:function(){var me=this,profile=this.getCurrentProfile(),launcher=this.getLaunch(),controllers,name;this.instantiateStores();Ext.app.Application.appInstance=this;if(Ext.Router){Ext.Router.setAppInstance(this);}
controllers=this.getControllerInstances();for(name in controllers){controllers[name].init(this);}
if(profile){profile.launch();}
launcher.call(me);for(name in controllers){if(controllers[name]&&!(controllers[name]instanceof Ext.app.Controller)){Ext.Logger.warn("The controller '"+name+"' doesn't have a launch method. Are you sure it extends from Ext.app.Controller?");}else{controllers[name].launch(this);}}
me.redirectTo(window.location.hash.substr(1));},gatherDependencies:function(){var classes=this.getModels().concat(this.getViews()).concat(this.getControllers());Ext.each(this.getStores(),function(storeName){if(Ext.isString(storeName)){classes.push(storeName);}},this);return classes;},instantiateStores:function(){var stores=this.getStores(),length=stores.length,store,storeClass,storeName,splits,i;for(i=0;i<length;i++){store=stores[i];if(Ext.data&&Ext.data.Store&&!(store instanceof Ext.data.Store)){if(Ext.isString(store)){storeName=store;storeClass=Ext.ClassManager.classes[store];store={xclass:store};if(storeClass.prototype.defaultConfig.storeId===undefined){splits=storeName.split('.');store.id=splits[splits.length-1];}}
stores[i]=Ext.factory(store,Ext.data.Store);}}
this.setStores(stores);},instantiateControllers:function(){var controllerNames=this.getControllers(),instances={},length=controllerNames.length,name,i;for(i=0;i<length;i++){name=controllerNames[i];instances[name]=Ext.create(name,{application:this});}
return this.setControllerInstances(instances);},applyControllers:function(controllers){return this.getFullyQualified(controllers,'controller');},applyProfiles:function(profiles){return this.getFullyQualified(profiles,'profile');},applyName:function(name){var oldName;if(name&&name.match(/ /g)){oldName=name;name=name.replace(/ /g,"");Ext.Logger.warn('Attempting to create an application with a name which contains whitespace ("'+oldName+'"). Renamed to "'+name+'".');}
return name;},updateName:function(newName){Ext.ClassManager.setNamespace(newName+'.app',this);if(!Ext.Loader.config.paths[newName]){Ext.Loader.setPath(newName,this.getAppFolder());}},applyRouter:function(config){return Ext.factory(config,Ext.app.Router,this.getRouter());},applyHistory:function(config){var history=Ext.factory(config,Ext.app.History,this.getHistory());history.on('change',this.onHistoryChange,this);return history;},onHistoryChange:function(url){this.dispatch(this.getRouter().recognize(url),false);}},function(){Ext.regApplication=function(config){Ext.Logger.deprecate('[Ext.app.Application] Ext.regApplication() is deprecated, please replace it with Ext.application()');var appName=config.name,format=Ext.String.format;Ext.ns(appName,format("{0}.controllers",appName),format("{0}.models",appName),format("{0}.views",appName));Ext.application(config);};Ext.define('Ext.data.ProxyMgr',{singleton:true,registerType:function(name,cls){Ext.Logger.deprecate('Ext.data.ProxyMgr no longer exists - instead of calling Ext.data.ProxyMgr.registerType just update '+'your custom Proxy class to set alias: "proxy.'+name+'"');Ext.ClassManager.setAlias(cls,"proxy."+name);}});Ext.reg=function(alias,cls){Ext.Logger.deprecate('Ext.reg is deprecated, please set xtype: "'+alias+'" directly in your subclass instead');Ext.ClassManager.setAlias(cls,alias);};Ext.redirect=function(){var app=Ext.app.Application.appInstance;Ext.Logger.deprecate('[Ext.app.Application] Ext.redirect is deprecated, please use YourApp.redirectTo instead');if(app){app.redirectTo.apply(app,arguments);}};Ext.dispatch=function(){var app=Ext.app.Application.appInstance;Ext.Logger.deprecate('[Ext.app.Application] Ext.dispatch is deprecated, please use YourApp.dispatch instead');if(app){app.dispatch.apply(app,arguments);}};});Ext.define('zvsMobile.profile.Phone',{extend:'Ext.app.Profile',config:{views:['Main','DevicePhoneViewPort','ScenePhoneViewPort','GroupPhoneViewPort']},isActive:function(){return Ext.os.is.Phone;},launch:function(){zvsMobile.tabPanel=Ext.create('zvsMobile.view.phone.Main');if(zvsMobile.app.BaseURL()!=''){zvsMobile.app.SetStoreProxys();Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/login',method:'GET',params:{u:Math.random()},success:function(response,opts){if(response.responseText!=''){var result=JSON.parse(response.responseText);if(result.success&&result.isLoggedIn){zvsMobile.app.SetStoreProxys();var settings=zvsMobile.tabPanel.items.items[4];settings.items.items[1].fireEvent('loggedIn');}
else{zvsMobile.app.fireEvent('ShowLoginScreen');}}
else{zvsMobile.app.fireEvent('ShowLoginScreen');}},failure:function(result,request){zvsMobile.app.fireEvent('ShowLoginScreen');}});}
else{zvsMobile.app.fireEvent('ShowLoginScreen');}}});Ext.define('zvsMobile.profile.Tablet',{extend:'Ext.app.Profile',config:{views:['Main','DeviceTabletViewPort','SceneTabletViewPort','GroupTabletViewPort']},isActive:function(){return!Ext.os.is.Phone;},launch:function(){zvsMobile.tabPanel=Ext.create('zvsMobile.view.tablet.Main');if(zvsMobile.app.BaseURL()!=''){Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/login',method:'GET',params:{u:Math.random()},success:function(response,opts){if(response.responseText!=''){var result=JSON.parse(response.responseText);if(result.success&&result.isLoggedIn){zvsMobile.app.SetStoreProxys();var settings=zvsMobile.tabPanel.items.items[4];settings.items.items[1].fireEvent('loggedIn');}
else{zvsMobile.app.fireEvent('ShowLoginScreen');}}
else{zvsMobile.app.fireEvent('ShowLoginScreen');}},failure:function(result,request){zvsMobile.app.fireEvent('ShowLoginScreen');}});}
else{zvsMobile.app.fireEvent('ShowLoginScreen');}}});Ext.define('Ext.mixin.Sortable',{extend:'Ext.mixin.Mixin',requires:['Ext.util.Sorter'],mixinConfig:{id:'sortable'},config:{sorters:null,defaultSortDirection:"ASC",sortRoot:null},dirtySortFn:false,sortFn:null,sorted:false,applySorters:function(sorters,collection){if(!collection){collection=this.createSortersCollection();}
collection.clear();this.sorted=false;if(sorters){this.addSorters(sorters);}
return collection;},createSortersCollection:function(){this._sorters=Ext.create('Ext.util.Collection',function(sorter){return sorter.getId();});return this._sorters;},addSorter:function(sorter,defaultDirection){this.addSorters([sorter],defaultDirection);},addSorters:function(sorters,defaultDirection){var currentSorters=this.getSorters();return this.insertSorters(currentSorters?currentSorters.length:0,sorters,defaultDirection);},insertSorter:function(index,sorter,defaultDirection){return this.insertSorters(index,[sorter],defaultDirection);},insertSorters:function(index,sorters,defaultDirection){if(!Ext.isArray(sorters)){sorters=[sorters];}
var ln=sorters.length,direction=defaultDirection||this.getDefaultSortDirection(),sortRoot=this.getSortRoot(),currentSorters=this.getSorters(),newSorters=[],sorterConfig,i,sorter,currentSorter;if(!currentSorters){currentSorters=this.createSortersCollection();}
for(i=0;i<ln;i++){sorter=sorters[i];sorterConfig={direction:direction,root:sortRoot};if(typeof sorter==='string'){currentSorter=currentSorters.get(sorter);if(!currentSorter){sorterConfig.property=sorter;}else{if(defaultDirection){currentSorter.setDirection(defaultDirection);}else{currentSorter.toggle();}
continue;}}
else if(Ext.isFunction(sorter)){sorterConfig.sorterFn=sorter;}
else if(Ext.isObject(sorter)){if(!sorter.isSorter){if(sorter.fn){sorter.sorterFn=sorter.fn;delete sorter.fn;}
sorterConfig=Ext.apply(sorterConfig,sorter);}
else{newSorters.push(sorter);if(!sorter.getRoot()){sorter.setRoot(sortRoot);}
continue;}}
else{Ext.Logger.warn('Invalid sorter specified:',sorter);}
sorter=Ext.create('Ext.util.Sorter',sorterConfig);newSorters.push(sorter);}
for(i=0,ln=newSorters.length;i<ln;i++){currentSorters.insert(index+i,newSorters[i]);}
this.dirtySortFn=true;if(currentSorters.length){this.sorted=true;}
return currentSorters;},removeSorter:function(sorter){return this.removeSorters([sorter]);},removeSorters:function(sorters){if(!Ext.isArray(sorters)){sorters=[sorters];}
var ln=sorters.length,currentSorters=this.getSorters(),i,sorter;for(i=0;i<ln;i++){sorter=sorters[i];if(typeof sorter==='string'){currentSorters.removeAtKey(sorter);}
else if(typeof sorter==='function'){currentSorters.each(function(item){if(item.getSorterFn()===sorter){currentSorters.remove(item);}});}
else if(sorter.isSorter){currentSorters.remove(sorter);}}
if(!currentSorters.length){this.sorted=false;}},updateSortFn:function(){var sorters=this.getSorters().items;this.sortFn=function(r1,r2){var ln=sorters.length,result,i;for(i=0;i<ln;i++){result=sorters[i].sort.call(this,r1,r2);if(result!==0){break;}}
return result;};this.dirtySortFn=false;return this.sortFn;},getSortFn:function(){if(this.dirtySortFn){return this.updateSortFn();}
return this.sortFn;},sort:function(data){Ext.Array.sort(data,this.getSortFn());return data;},findInsertionIndex:function(items,item,sortFn){var start=0,end=items.length-1,sorterFn=sortFn||this.getSortFn(),middle,comparison;while(start<=end){middle=(start+end)>>1;comparison=sorterFn(item,items[middle]);if(comparison>=0){start=middle+1;}else if(comparison<0){end=middle-1;}}
return start;}});Ext.define('Ext.mixin.Filterable',{extend:'Ext.mixin.Mixin',requires:['Ext.util.Filter'],mixinConfig:{id:'filterable'},config:{filters:null,filterRoot:null},dirtyFilterFn:false,filterFn:null,filtered:false,applyFilters:function(filters,collection){if(!collection){collection=this.createFiltersCollection();}
collection.clear();this.filtered=false;this.dirtyFilterFn=true;if(filters){this.addFilters(filters);}
return collection;},createFiltersCollection:function(){this._filters=Ext.create('Ext.util.Collection',function(filter){return filter.getId();});return this._filters;},addFilter:function(filter){this.addFilters([filter]);},addFilters:function(filters){var currentFilters=this.getFilters();return this.insertFilters(currentFilters?currentFilters.length:0,filters);},insertFilter:function(index,filter){return this.insertFilters(index,[filter]);},insertFilters:function(index,filters){if(!Ext.isArray(filters)){filters=[filters];}
var ln=filters.length,filterRoot=this.getFilterRoot(),currentFilters=this.getFilters(),newFilters=[],filterConfig,i,filter;if(!currentFilters){currentFilters=this.createFiltersCollection();}
for(i=0;i<ln;i++){filter=filters[i];filterConfig={root:filterRoot};if(Ext.isFunction(filter)){filterConfig.filterFn=filter;}
else if(Ext.isObject(filter)){if(!filter.isFilter){if(filter.fn){filter.filterFn=filter.fn;delete filter.fn;}
filterConfig=Ext.apply(filterConfig,filter);}
else{newFilters.push(filter);if(!filter.getRoot()){filter.setRoot(filterRoot);}
continue;}}
else{Ext.Logger.warn('Invalid filter specified:',filter);}
filter=Ext.create('Ext.util.Filter',filterConfig);newFilters.push(filter);}
for(i=0,ln=newFilters.length;i<ln;i++){currentFilters.insert(index+i,newFilters[i]);}
this.dirtyFilterFn=true;if(currentFilters.length){this.filtered=true;}
return currentFilters;},removeFilters:function(filters){if(!Ext.isArray(filters)){filters=[filters];}
var ln=filters.length,currentFilters=this.getFilters(),i,filter;for(i=0;i<ln;i++){filter=filters[i];if(typeof filter==='string'){currentFilters.each(function(item){if(item.getProperty()===filter){currentFilters.remove(item);}});}
else if(typeof filter==='function'){currentFilters.each(function(item){if(item.getFilterFn()===filter){currentFilters.remove(item);}});}
else{if(filter.isFilter){currentFilters.remove(filter);}
else if(filter.property!==undefined&&filter.value!==undefined){currentFilters.each(function(item){if(item.getProperty()===filter.property&&item.getValue()===filter.value){currentFilters.remove(item);}});}}}
if(!currentFilters.length){this.filtered=false;}},updateFilterFn:function(){var filters=this.getFilters().items;this.filterFn=function(item){var isMatch=true,length=filters.length,i;for(i=0;i<length;i++){var filter=filters[i],fn=filter.getFilterFn(),scope=filter.getScope()||this;isMatch=isMatch&&fn.call(scope,item);}
return isMatch;};this.dirtyFilterFn=false;return this.filterFn;},filter:function(data){return this.getFilters().length?Ext.Array.filter(data,this.getFilterFn()):data;},isFiltered:function(item){return this.getFilters().length?!this.getFilterFn()(item):false;},getFilterFn:function(){if(this.dirtyFilterFn){return this.updateFilterFn();}
return this.filterFn;}});Ext.define('Ext.util.Collection',{config:{autoFilter:true,autoSort:true},mixins:{sortable:'Ext.mixin.Sortable',filterable:'Ext.mixin.Filterable'},constructor:function(keyFn,config){var me=this;me.all=[];me.items=[];me.keys=[];me.indices={};me.map={};me.length=0;if(keyFn){me.getKey=keyFn;}
this.initConfig(config);},updateAutoSort:function(autoSort,oldAutoSort){if(oldAutoSort===false&&autoSort&&this.items.length){this.sort();}},updateAutoFilter:function(autoFilter,oldAutoFilter){if(oldAutoFilter===false&&autoFilter&&this.all.length){this.runFilters();}},insertSorters:function(){this.mixins.sortable.insertSorters.apply(this,arguments);if(this.getAutoSort()&&this.items.length){this.sort();}
return this;},removeSorters:function(sorters){this.mixins.sortable.removeSorters.call(this,sorters);if(this.sorted&&this.getAutoSort()&&this.items.length){this.sort();}
return this;},applyFilters:function(filters){var collection=this.mixins.filterable.applyFilters.call(this,filters);if(!filters&&this.all.length&&this.getAutoFilter()){this.filter();}
return collection;},addFilters:function(filters){this.mixins.filterable.addFilters.call(this,filters);if(this.items.length&&this.getAutoFilter()){this.runFilters();}
return this;},removeFilters:function(filters){this.mixins.filterable.removeFilters.call(this,filters);if(this.filtered&&this.all.length&&this.getAutoFilter()){this.filter();}
return this;},filter:function(property,value,anyMatch,caseSensitive){if(property){if(Ext.isString(property)){this.addFilters({property:property,value:value,anyMatch:anyMatch,caseSensitive:caseSensitive});return this.items;}
else{this.addFilters(property);return this.items;}}
this.items=this.mixins.filterable.filter.call(this,this.all.slice());this.updateAfterFilter();if(this.sorted&&this.getAutoSort()){this.sort();}},runFilters:function(){this.items=this.mixins.filterable.filter.call(this,this.items);this.updateAfterFilter();},updateAfterFilter:function(){var items=this.items,keys=this.keys,indices=this.indices={},ln=items.length,i,item,key;keys.length=0;for(i=0;i<ln;i++){item=items[i];key=this.getKey(item);indices[key]=i;keys[i]=key;}
this.length=items.length;this.dirtyIndices=false;},sort:function(sorters,defaultDirection){var items=this.items,keys=this.keys,indices=this.indices,ln=items.length,i,item,key;if(sorters){this.addSorters(sorters,defaultDirection);return this.items;}
for(i=0;i<ln;i++){items[i]._current_key=keys[i];}
this.handleSort(items);for(i=0;i<ln;i++){item=items[i];key=item._current_key;keys[i]=key;indices[key]=i;delete item._current_key;}
this.dirtyIndices=true;},handleSort:function(items){this.mixins.sortable.sort.call(this,items);},add:function(key,item){var me=this,filtered=this.filtered,sorted=this.sorted,all=this.all,items=this.items,keys=this.keys,indices=this.indices,filterable=this.mixins.filterable,currentLength=items.length,index=currentLength;if(arguments.length==1){item=key;key=me.getKey(item);}
if(typeof key!='undefined'&&key!==null){if(typeof me.map[key]!='undefined'){return me.replace(key,item);}
me.map[key]=item;}
all.push(item);if(filtered&&this.getAutoFilter()&&filterable.isFiltered.call(me,item)){return null;}
me.length++;if(sorted&&this.getAutoSort()){index=this.findInsertionIndex(items,item);}
if(index!==currentLength){this.dirtyIndices=true;Ext.Array.splice(keys,index,0,key);Ext.Array.splice(items,index,0,item);}else{indices[key]=currentLength;keys.push(key);items.push(item);}
return item;},getKey:function(item){return item.id;},replace:function(oldKey,item){var me=this,sorted=me.sorted,filtered=me.filtered,filterable=me.mixins.filterable,items=me.items,keys=me.keys,all=me.all,map=me.map,returnItem=null,oldItemsLn=items.length,oldItem,index,newKey;if(arguments.length==1){item=oldKey;oldKey=newKey=me.getKey(item);}else{newKey=me.getKey(item);}
oldItem=map[oldKey];if(typeof oldKey=='undefined'||oldKey===null||typeof oldItem=='undefined'){return me.add(newKey,item);}
me.map[newKey]=item;if(newKey!==oldKey){delete me.map[oldKey];}
if(sorted&&me.getAutoSort()){Ext.Array.remove(items,oldItem);Ext.Array.remove(keys,oldKey);Ext.Array.remove(all,oldItem);all.push(item);me.dirtyIndices=true;if(filtered&&me.getAutoFilter()){if(filterable.isFiltered.call(me,item)){if(oldItemsLn!==items.length){me.length--;}
return null;}
else if(oldItemsLn===items.length){me.length++;returnItem=item;}}
index=this.findInsertionIndex(items,item);Ext.Array.splice(keys,index,0,newKey);Ext.Array.splice(items,index,0,item);}else{if(filtered){if(me.getAutoFilter()&&filterable.isFiltered.call(me,item)){if(items.indexOf(oldItem)!==-1){Ext.Array.remove(items,oldItem);Ext.Array.remove(keys,oldKey);me.length--;me.dirtyIndices=true;}
return null;}
else if(items.indexOf(oldItem)===-1){items.push(item);keys.push(newKey);me.indices[newKey]=me.length;me.length++;return item;}}
index=me.items.indexOf(oldItem);keys[index]=newKey;items[index]=item;this.dirtyIndices=true;}
return returnItem;},addAll:function(addItems){var me=this,filtered=me.filtered,sorted=me.sorted,all=me.all,items=me.items,keys=me.keys,map=me.map,autoFilter=me.getAutoFilter(),autoSort=me.getAutoSort(),newKeys=[],newItems=[],filterable=me.mixins.filterable,addedItems=[],ln,key,i,item;if(Ext.isObject(addItems)){for(key in addItems){if(addItems.hasOwnProperty(key)){newItems.push(items[key]);newKeys.push(key);}}}else{newItems=addItems;ln=addItems.length;for(i=0;i<ln;i++){newKeys.push(me.getKey(addItems[i]));}}
for(i=0;i<ln;i++){key=newKeys[i];item=newItems[i];if(typeof key!='undefined'&&key!==null){if(typeof map[key]!='undefined'){me.replace(key,item);continue;}
map[key]=item;}
all.push(item);if(filtered&&autoFilter&&filterable.isFiltered.call(me,item)){continue;}
me.length++;keys.push(key);items.push(item);addedItems.push(item);}
if(addedItems.length){me.dirtyIndices=true;if(sorted&&autoSort){me.sort();}
return addedItems;}
return null;},each:function(fn,scope){var items=this.items.slice(),i=0,len=items.length,item;for(;i<len;i++){item=items[i];if(fn.call(scope||item,item,i,len)===false){break;}}},eachKey:function(fn,scope){var keys=this.keys,items=this.items,ln=keys.length,i;for(i=0;i<ln;i++){fn.call(scope||window,keys[i],items[i],i,ln);}},findBy:function(fn,scope){var keys=this.keys,items=this.items,i=0,len=items.length;for(;i<len;i++){if(fn.call(scope||window,items[i],keys[i])){return items[i];}}
return null;},filterBy:function(fn,scope){var me=this,newCollection=new this.self(),keys=me.keys,items=me.all,length=items.length,i;newCollection.getKey=me.getKey;for(i=0;i<length;i++){if(fn.call(scope||me,items[i],keys[i])){newCollection.add(keys[i],items[i]);}}
return newCollection;},insert:function(index,key,item){var me=this,sorted=this.sorted,filtered=this.filtered;if(arguments.length==2){item=key;key=me.getKey(item);}
if(me.containsKey(key)){me.removeAtKey(key);}
if(index>=me.length||(sorted&&me.getAutoSort())){return me.add(key,item);}
this.all.push(item);if(typeof key!='undefined'&&key!==null){me.map[key]=item;}
if(filtered&&this.getAutoFilter()&&this.mixins.filterable.isFiltered.call(me,item)){return null;}
me.length++;Ext.Array.splice(me.items,index,0,item);Ext.Array.splice(me.keys,index,0,key);me.dirtyIndices=true;return item;},insertAll:function(index,insertItems){if(index>=this.items.length||(this.sorted&&this.getAutoSort())){return this.addAll(insertItems);}
var me=this,filtered=this.filtered,sorted=this.sorted,all=this.all,items=this.items,keys=this.keys,map=this.map,autoFilter=this.getAutoFilter(),autoSort=this.getAutoSort(),newKeys=[],newItems=[],addedItems=[],filterable=this.mixins.filterable,insertedUnfilteredItem=false,ln,key,i,item;if(sorted&&this.getAutoSort()){Ext.Logger.error('Inserting a collection of items into a sorted Collection is invalid. Please just add these items or remove the sorters.');}
if(Ext.isObject(insertItems)){for(key in insertItems){if(insertItems.hasOwnProperty(key)){newItems.push(items[key]);newKeys.push(key);}}}else{newItems=insertItems;ln=insertItems.length;for(i=0;i<ln;i++){newKeys.push(me.getKey(insertItems[i]));}}
for(i=0;i<ln;i++){key=newKeys[i];item=newItems[i];if(typeof key!='undefined'&&key!==null){if(me.containsKey(key)){me.removeAtKey(key);}
map[key]=item;}
all.push(item);if(filtered&&autoFilter&&filterable.isFiltered.call(me,item)){continue;}
me.length++;Ext.Array.splice(items,index+i,0,item);Ext.Array.splice(keys,index+i,0,key);insertedUnfilteredItem=true;addedItems.push(item);}
if(insertedUnfilteredItem){this.dirtyIndices=true;if(sorted&&autoSort){this.sort();}
return addedItems;}
return null;},remove:function(item){var index=this.items.indexOf(item);if(index===-1){Ext.Array.remove(this.all,item);return item;}
return this.removeAt(this.items.indexOf(item));},removeAll:function(items){if(items){var ln=items.length,i;for(i=0;i<ln;i++){this.remove(items[i]);}}
return this;},removeAt:function(index){var me=this,items=me.items,keys=me.keys,all=this.all,item,key;if(index<me.length&&index>=0){item=items[index];key=keys[index];if(typeof key!='undefined'){delete me.map[key];}
Ext.Array.erase(items,index,1);Ext.Array.erase(keys,index,1);Ext.Array.remove(all,item);me.length--;this.dirtyIndices=true;return item;}
return false;},removeAtKey:function(key){return this.removeAt(this.indexOfKey(key));},getCount:function(){return this.length;},indexOf:function(item){if(this.dirtyIndices){this.updateIndices();}
var index=this.indices[this.getKey(item)];return(index===undefined)?-1:index;},indexOfKey:function(key){if(this.dirtyIndices){this.updateIndices();}
var index=this.indices[key];return(index===undefined)?-1:index;},updateIndices:function(){var items=this.items,ln=items.length,indices=this.indices={},i,item,key;for(i=0;i<ln;i++){item=items[i];key=this.getKey(item);indices[key]=i;}
this.dirtyIndices=false;},get:function(key){var me=this,fromMap=me.map[key],item;if(fromMap!==undefined){item=fromMap;}
else if(typeof key=='number'){item=me.items[key];}
return typeof item!='function'||me.getAllowFunctions()?item:null;},getAt:function(index){return this.items[index];},getByKey:function(key){return this.map[key];},contains:function(item){var key=this.getKey(item);if(key){return this.containsKey(key);}else{return Ext.Array.contains(this.items,item);}},containsKey:function(key){return typeof this.map[key]!='undefined';},clear:function(){var me=this;me.length=0;me.items.length=0;me.keys.length=0;me.all.length=0;me.map={};},first:function(){return this.items[0];},last:function(){return this.items[this.length-1];},getRange:function(start,end){var me=this,items=me.items,range=[],i;if(items.length<1){return range;}
start=start||0;end=Math.min(typeof end=='undefined'?me.length-1:end,me.length-1);if(start<=end){for(i=start;i<=end;i++){range[range.length]=items[i];}}else{for(i=start;i>=end;i--){range[range.length]=items[i];}}
return range;},findIndexBy:function(fn,scope,start){var me=this,keys=me.keys,items=me.items,i=start||0,ln=items.length;for(;i<ln;i++){if(fn.call(scope||me,items[i],keys[i])){return i;}}
return-1;},clone:function(){var me=this,copy=new this.self(),keys=me.keys,items=me.items,i=0,ln=items.length;for(;i<ln;i++){copy.add(keys[i],items[i]);}
copy.getKey=me.getKey;return copy;}});Ext.define('Ext.data.Operation',{config:{synchronous:true,action:null,filters:null,sorters:null,grouper:null,start:null,limit:null,batch:null,callback:null,scope:null,resultSet:null,records:null,request:null,response:null,withCredentials:null,params:null,url:null,page:null,node:null,model:undefined,addRecords:false},started:false,running:false,complete:false,success:undefined,exception:false,error:undefined,constructor:function(config){this.initConfig(config);},applyModel:function(model){if(typeof model=='string'){model=Ext.data.ModelManager.getModel(model);if(!model){Ext.Logger.error('Model with name '+arguments[0]+' doesnt exist.');}}
if(model&&!model.prototype.isModel&&Ext.isObject(model)){model=Ext.data.ModelManager.registerType(model.storeId||model.id||Ext.id(),model);}
if(!model){Ext.Logger.warn('Unless you define your model using metadata, an Operation needs to have a model defined.');}
return model;},getRecords:function(){var resultSet=this.getResultSet();return this._records||(resultSet?resultSet.getRecords():[]);},setStarted:function(){this.started=true;this.running=true;},setCompleted:function(){this.complete=true;this.running=false;},setSuccessful:function(){this.success=true;},setException:function(error){this.exception=true;this.success=false;this.running=false;this.error=error;},hasException:function(){return this.exception===true;},getError:function(){return this.error;},isStarted:function(){return this.started===true;},isRunning:function(){return this.running===true;},isComplete:function(){return this.complete===true;},wasSuccessful:function(){return this.isComplete()&&this.success===true;},allowWrite:function(){return this.getAction()!='read';},process:function(action,resultSet,request,response){if(resultSet.getSuccess()!==false){this.setResponse(response);this.setResultSet(resultSet);this.setCompleted();this.setSuccessful();}else{return false;}
return this['process'+Ext.String.capitalize(action)].call(this,resultSet,request,response);},processRead:function(resultSet){var records=resultSet.getRecords(),processedRecords=[],Model=this.getModel(),ln=records.length,i,record;for(i=0;i<ln;i++){record=records[i];processedRecords.push(new Model(record.data,record.id,record.node));}
this.setRecords(processedRecords);return true;},processCreate:function(resultSet){var updatedRecords=resultSet.getRecords(),currentRecords=this.getRecords(),ln=updatedRecords.length,i,currentRecord,updatedRecord;for(i=0;i<ln;i++){updatedRecord=updatedRecords[i];if(updatedRecord.clientId===null&&currentRecords.length==1&&updatedRecords.length==1){currentRecord=currentRecords[i];}else{currentRecord=this.findCurrentRecord(updatedRecord.clientId);}
if(currentRecord){this.updateRecord(currentRecord,updatedRecord);}
else{Ext.Logger.warn('Unable to match the record that came back from the server.');}}
return true;},processUpdate:function(resultSet){var updatedRecords=resultSet.getRecords(),currentRecords=this.getRecords(),ln=updatedRecords.length,i,currentRecord,updatedRecord;for(i=0;i<ln;i++){updatedRecord=updatedRecords[i];currentRecord=currentRecords[i];if(currentRecord){this.updateRecord(currentRecord,updatedRecord);}
else{Ext.Logger.warn('Unable to match the updated record that came back from the server.');}}
return true;},processDestroy:function(resultSet){var updatedRecords=resultSet.getRecords(),ln=updatedRecords.length,i,currentRecord,updatedRecord;for(i=0;i<ln;i++){updatedRecord=updatedRecords[i];currentRecord=this.findCurrentRecord(updatedRecord.id);if(currentRecord){currentRecord.setIsErased(true);currentRecord.notifyStores('afterErase',currentRecord);}
else{Ext.Logger.warn('Unable to match the destroyed record that came back from the server.');}}},findCurrentRecord:function(clientId){var currentRecords=this.getRecords(),ln=currentRecords.length,i,currentRecord;for(i=0;i<ln;i++){currentRecord=currentRecords[i];if(currentRecord.getId()===clientId){return currentRecord;}}},updateRecord:function(currentRecord,updatedRecord){var recordData=updatedRecord.data,recordId=updatedRecord.id;currentRecord.beginEdit();currentRecord.set(recordData);if(recordId!==null){currentRecord.setId(recordId);}
currentRecord.endEdit(true);currentRecord.commit();}},function(){Ext.deprecateProperty(this,'group','grouper');});Ext.define('Ext.data.ResultSet',{config:{loaded:true,count:null,total:null,success:false,records:null,message:null},constructor:function(config){this.initConfig(config);},applyCount:function(count){if(!count&&count!==0){return this.getRecords().length;}
return count;},updateRecords:function(records){this.setCount(records.length);}});Ext.define('Ext.data.reader.Reader',{requires:['Ext.data.ResultSet'],alternateClassName:['Ext.data.Reader','Ext.data.DataReader'],mixins:['Ext.mixin.Observable'],isReader:true,config:{idProperty:undefined,clientIdProperty:'clientId',totalProperty:'total',successProperty:'success',messageProperty:null,rootProperty:'',implicitIncludes:true,model:undefined},constructor:function(config){this.initConfig(config);},fieldCount:0,applyModel:function(model){if(typeof model=='string'){model=Ext.data.ModelManager.getModel(model);if(!model){Ext.Logger.error('Model with name '+arguments[0]+' doesnt exist.');}}
if(model&&!model.prototype.isModel&&Ext.isObject(model)){model=Ext.data.ModelManager.registerType(model.storeId||model.id||Ext.id(),model);}
return model;},applyIdProperty:function(idProperty){if(!idProperty&&this.getModel()){idProperty=this.getModel().getIdProperty();}
return idProperty;},updateModel:function(model){if(model){if(!this.getIdProperty()){this.setIdProperty(model.getIdProperty());}
this.buildExtractors();}},createAccessor:Ext.emptyFn,createFieldAccessExpression:function(){return'undefined';},buildExtractors:function(){if(!this.getModel()){return;}
var me=this,totalProp=me.getTotalProperty(),successProp=me.getSuccessProperty(),messageProp=me.getMessageProperty();if(totalProp){me.getTotal=me.createAccessor(totalProp);}
if(successProp){me.getSuccess=me.createAccessor(successProp);}
if(messageProp){me.getMessage=me.createAccessor(messageProp);}
me.extractRecordData=me.buildRecordDataExtractor();},buildRecordDataExtractor:function(){var me=this,model=me.getModel(),fields=model.getFields(),ln=fields.length,fieldVarName=[],clientIdProp=me.getModel().getClientIdProperty(),prefix='__field',code=['var me = this,\n','    fields = me.getModel().getFields(),\n','    idProperty = me.getIdProperty(),\n','    idPropertyIsFn = (typeof idProperty == "function"),','    value,\n','    internalId'],i,field,varName,fieldName;fields=fields.items;for(i=0;i<ln;i++){field=fields[i];fieldName=field.getName();if(fieldName===model.getIdProperty()){fieldVarName[i]='idField';}else{fieldVarName[i]=prefix+i;}
code.push(',\n    ',fieldVarName[i],' = fields.get("',field.getName(),'")');}
code.push(';\n\n    return function(source) {\n        var dest = {};\n');code.push('        if (idPropertyIsFn) {\n');code.push('            idField.setMapping(idProperty);\n');code.push('        }\n');for(i=0;i<ln;i++){field=fields[i];varName=fieldVarName[i];fieldName=field.getName();if(fieldName===model.getIdProperty()&&field.getMapping()===null&&model.getIdProperty()!==this.getIdProperty()){field.setMapping(this.getIdProperty());}
code.push('        try {\n');code.push('            value = ',me.createFieldAccessExpression(field,varName,'source'),';\n');code.push('            if (value !== undefined) {\n');code.push('                dest["'+field.getName()+'"] = value;\n');code.push('            }\n');code.push('        } catch(e){}\n');}
if(clientIdProp){code.push('        internalId = '+me.createFieldAccessExpression(Ext.create('Ext.data.Field',{name:clientIdProp}),null,'source')+';\n');code.push('        if (internalId !== undefined) {\n');code.push('            dest["_clientId"] = internalId;\n        }\n');}
code.push('        return dest;\n');code.push('    };');return Ext.functionFactory(code.join('')).call(me);},getFields:function(){return this.getModel().getFields().items;},getData:function(data){return data;},getResponseData:function(response){return response;},getRoot:function(data){return data;},read:function(response){var data=response,Model=this.getModel(),resultSet,records,i,ln,record;if(response){data=this.getResponseData(response);}
if(data){resultSet=this.readRecords(data);records=resultSet.getRecords();for(i=0,ln=records.length;i<ln;i++){record=records[i];records[i]=new Model(record.data,record.id,record.node);}
return resultSet;}else{return this.nullResultSet;}},process:function(response){var data=response;if(response){data=this.getResponseData(response);}
if(data){return this.readRecords(data);}else{return this.nullResultSet;}},readRecords:function(data){var me=this;me.rawData=data;data=me.getData(data);if(data.metaData){me.onMetaChange(data.metaData);}
if(!me.getModel()){Ext.Logger.warn('In order to read record data, a Reader needs to have a Model defined on it.');}
var root=Ext.isArray(data)?data:me.getRoot(data),success=true,recordCount=0,total,value,records,message;if(me.getTotalProperty()){value=parseInt(me.getTotal(data),10);if(!isNaN(value)){total=value;}}
if(me.getSuccessProperty()){value=me.getSuccess(data);if(value===false||value==='false'){success=false;}}
if(me.getMessageProperty()){message=me.getMessage(data);}
if(root){records=me.extractData(root);recordCount=records.length;}else{recordCount=0;records=[];}
return new Ext.data.ResultSet({total:total,count:recordCount,records:records,success:success,message:message});},extractData:function(root){var me=this,records=[],length=root.length,model=me.getModel(),idProperty=model.getIdProperty(),fieldsCollection=model.getFields(),node,i,data,id,clientId;if(fieldsCollection.isDirty){me.buildExtractors(true);delete fieldsCollection.isDirty;}
if(!root.length&&Ext.isObject(root)){root=[root];length=1;}
for(i=0;i<length;i++){clientId=null;id=null;node=root[i];if(node.isModel){data=node.data;}else{data=me.extractRecordData(node);}
if(data._clientId!==undefined){clientId=data._clientId;delete data._clientId;}
if(data[idProperty]!==undefined){id=data[idProperty];}
if(me.getImplicitIncludes()){me.readAssociated(data,node);}
records.push({clientId:clientId,id:id,data:data,node:node});}
return records;},readAssociated:function(data,node){var associations=this.getModel().associations.items,i=0,length=associations.length,association,associationData,associationKey;for(;i<length;i++){association=associations[i];associationKey=association.getAssociationKey();associationData=this.getAssociatedDataRoot(node,associationKey);if(associationData){data[associationKey]=associationData;}}},getAssociatedDataRoot:function(data,associationName){return data[associationName];},onMetaChange:function(meta){var fields=meta.fields,me=this,newModel,config,idProperty;me.metaData=meta;if(meta.rootProperty!==undefined){me.setRootProperty(meta.rootProperty);}
else if(meta.root!==undefined){me.setRootProperty(meta.root);}
if(meta.idProperty!==undefined){me.setIdProperty(meta.idProperty);}
if(meta.totalProperty!==undefined){me.setTotalProperty(meta.totalProperty);}
if(meta.successProperty!==undefined){me.setSuccessProperty(meta.successProperty);}
if(meta.messageProperty!==undefined){me.setMessageProperty(meta.messageProperty);}
if(fields){if(me.getModel()){me.getModel().setFields(fields);me.buildExtractors();}
else{idProperty=me.getIdProperty();config={fields:fields};if(idProperty){config.idProperty=idProperty;}
newModel=Ext.define("Ext.data.reader.MetaModel"+Ext.id(),{extend:'Ext.data.Model',config:config});me.setModel(newModel);}}
else{me.buildExtractors();}},onClassExtended:function(cls,data,hooks){var Component=this,defaultConfig=Component.prototype.config,config=data.config||{},key;for(key in defaultConfig){if(key in data){config[key]=data[key];delete data[key];Ext.Logger.deprecate(key+' is deprecated as a property directly on the Reader prototype. '+'Please put it inside the config object.');}}
data.config=config;}},function(){Ext.apply(this.prototype,{nullResultSet:new Ext.data.ResultSet({total:0,count:0,records:[],success:false})});this.override({constructor:function(config){config=config||{};if(config.root){Ext.Logger.deprecate('root has been deprecated as a configuration on Reader. Please use rootProperty instead.');config.rootProperty=config.root;delete config.root;}
this.callOverridden([config]);}});});Ext.define('Ext.data.reader.Json',{extend:'Ext.data.reader.Reader',alternateClassName:'Ext.data.JsonReader',alias:'reader.json',config:{record:null,useSimpleAccessors:false},objectRe:/[\[\.]/,getResponseData:function(response){var responseText=response;if(response&&response.responseText){responseText=response.responseText;}
if(typeof responseText!=='string'){return responseText;}
var data;try{data=Ext.decode(responseText);}
catch(ex){this.fireEvent('exception',this,response,'Unable to parse the JSON returned by the server: '+ex.toString());Ext.Logger.warn('Unable to parse the JSON returned by the server: '+ex.toString());}
if(!data){this.fireEvent('exception',this,response,'JSON object not found');Ext.Logger.error('JSON object not found');}
return data;},buildExtractors:function(){var me=this,root=me.getRootProperty();me.callParent(arguments);if(root){me.rootAccessor=me.createAccessor(root);}else{delete me.rootAccessor;}},getRoot:function(data){var fieldsCollection=this.getModel().getFields();if(fieldsCollection.isDirty){this.buildExtractors(true);delete fieldsCollection.isDirty;}
if(this.rootAccessor){return this.rootAccessor.call(this,data);}else{return data;}},extractData:function(root){var recordName=this.getRecord(),data=[],length,i;if(recordName){length=root.length;if(!length&&Ext.isObject(root)){length=1;root=[root];}
for(i=0;i<length;i++){data[i]=root[i][recordName];}}else{data=root;}
return this.callParent([data]);},createAccessor:function(){var re=/[\[\.]/;return function(expr){if(Ext.isEmpty(expr)){return Ext.emptyFn;}
if(Ext.isFunction(expr)){return expr;}
if(this.getUseSimpleAccessors()!==true){var i=String(expr).search(re);if(i>=0){return Ext.functionFactory('obj','var value; try {value = obj'+(i>0?'.':'')+expr+'} catch(e) {}; return value;');}}
return function(obj){return obj[expr];};};}(),createFieldAccessExpression:function(field,fieldVarName,dataName){var me=this,re=me.objectRe,hasMap=(field.getMapping()!==null),map=hasMap?field.getMapping():field.getName(),result,operatorSearch;if(typeof map==='function'){result=fieldVarName+'.getMapping()('+dataName+', this)';}
else if(me.getUseSimpleAccessors()===true||((operatorSearch=String(map).search(re))<0)){if(!hasMap||isNaN(map)){map='"'+map+'"';}
result=dataName+"["+map+"]";}
else{result=dataName+(operatorSearch>0?'.':'')+map;}
return result;}});Ext.define('Ext.data.writer.Writer',{alias:'writer.base',alternateClassName:['Ext.data.DataWriter','Ext.data.Writer'],config:{writeAllFields:true,nameProperty:'name'},constructor:function(config){this.initConfig(config);},write:function(request){var operation=request.getOperation(),records=operation.getRecords()||[],len=records.length,i=0,data=[];for(;i<len;i++){data.push(this.getRecordData(records[i]));}
return this.writeRecords(request,data);},writeDate:function(field,date){var dateFormat=field.dateFormat||'timestamp';switch(dateFormat){case'timestamp':return date.getTime()/1000;case'time':return date.getTime();default:return Ext.Date.format(date,dateFormat);}},getRecordData:function(record){var isPhantom=record.phantom===true,writeAll=this.getWriteAllFields()||isPhantom,nameProperty=this.getNameProperty(),fields=record.getFields(),data={},changes,name,field,key,value,fieldConfig;if(writeAll){fields.each(function(field){fieldConfig=field.config;if(fieldConfig.persist){name=fieldConfig[nameProperty]||fieldConfig.name;value=record.get(fieldConfig.name);if(fieldConfig.type.type=='date'){value=this.writeDate(fieldConfig,value);}
data[name]=value;}},this);}else{changes=record.getChanges();for(key in changes){if(changes.hasOwnProperty(key)){field=fields.get(key);fieldConfig=field.config;if(fieldConfig.persist){name=fieldConfig[nameProperty]||field.name;value=changes[key];if(fieldConfig.type.type=='date'){value=this.writeDate(fieldConfig,value);}
data[name]=value;}}}
if(!isPhantom){data[record.getIdProperty()]=record.getId();}}
return data;},onClassExtended:function(cls,data,hooks){var Component=this,defaultConfig=Component.prototype.config,config=data.config||{},key;for(key in defaultConfig){if(key in data){config[key]=data[key];delete data[key];Ext.Logger.deprecate(key+' is deprecated as a property directly on the Writer prototype. '+'Please put it inside the config object.');}}
data.config=config;}});Ext.define('Ext.data.writer.Json',{extend:'Ext.data.writer.Writer',alternateClassName:'Ext.data.JsonWriter',alias:'writer.json',config:{root:undefined,encode:false,allowSingle:true,encodeRequest:false},applyRoot:function(root){if(!root&&(this.getEncode()||this.getEncodeRequest())){root='data';}
return root;},writeRecords:function(request,data){var root=this.getRoot(),params=request.getParams(),allowSingle=this.getAllowSingle(),jsonData;if(this.getAllowSingle()&&data&&data.length==1){data=data[0];}
if(this.getEncodeRequest()){jsonData=request.getJsonData()||{};if(data&&(data.length||(allowSingle&&Ext.isObject(data)))){jsonData[root]=data;}
request.setJsonData(Ext.apply(jsonData,params||{}));request.setParams(null);request.setMethod('POST');return request;}
if(!data||!(data.length||(allowSingle&&Ext.isObject(data)))){return request;}
if(this.getEncode()){if(root){params[root]=Ext.encode(data);}else{Ext.Logger.error('Must specify a root when using encode');}}else{jsonData=request.getJsonData()||{};if(root){jsonData[root]=data;}else{jsonData=data;}
request.setJsonData(jsonData);}
return request;}});Ext.define('Ext.data.Batch',{mixins:{observable:'Ext.mixin.Observable'},config:{autoStart:false,pauseOnException:true,proxy:null},current:-1,total:0,isRunning:false,isComplete:false,hasException:false,constructor:function(config){var me=this;me.initConfig(config);me.operations=[];},add:function(operation){this.total++;operation.setBatch(this);this.operations.push(operation);},start:function(){this.hasException=false;this.isRunning=true;this.runNextOperation();},runNextOperation:function(){this.runOperation(this.current+1);},pause:function(){this.isRunning=false;},runOperation:function(index){var me=this,operations=me.operations,operation=operations[index],onProxyReturn;if(operation===undefined){me.isRunning=false;me.isComplete=true;me.fireEvent('complete',me,operations[operations.length-1]);}else{me.current=index;onProxyReturn=function(operation){var hasException=operation.hasException();if(hasException){me.hasException=true;me.fireEvent('exception',me,operation);}else{me.fireEvent('operationcomplete',me,operation);}
if(hasException&&me.getPauseOnException()){me.pause();}else{operation.setCompleted();me.runNextOperation();}};operation.setStarted();me.getProxy()[operation.getAction()](operation,onProxyReturn,me);}}});Ext.define('Ext.data.proxy.Proxy',{extend:'Ext.Evented',alias:'proxy.proxy',alternateClassName:['Ext.data.DataProxy','Ext.data.Proxy'],requires:['Ext.data.reader.Json','Ext.data.writer.Json','Ext.data.Batch','Ext.data.Operation'],config:{batchOrder:'create,update,destroy',batchActions:true,model:null,reader:{type:'json'},writer:{type:'json'}},isProxy:true,applyModel:function(model){if(typeof model=='string'){model=Ext.data.ModelManager.getModel(model);if(!model){Ext.Logger.error('Model with name '+arguments[0]+' doesnt exist.');}}
if(model&&!model.prototype.isModel&&Ext.isObject(model)){model=Ext.data.ModelManager.registerType(model.storeId||model.id||Ext.id(),model);}
return model;},updateModel:function(model){if(model){var reader=this.getReader();if(reader&&!reader.getModel()){reader.setModel(model);}}},applyReader:function(reader,currentReader){return Ext.factory(reader,Ext.data.Reader,currentReader,'reader');},updateReader:function(reader){if(reader){var model=this.getModel();if(!model){model=reader.getModel();if(model){this.setModel(model);}}else{reader.setModel(model);}
if(reader.onMetaChange){reader.onMetaChange=Ext.Function.createSequence(reader.onMetaChange,this.onMetaChange,this);}}},onMetaChange:function(data){var model=this.getReader().getModel();if(!this.getModel()&&model){this.setModel(model);}
this.fireEvent('metachange',this,data);},applyWriter:function(writer,currentWriter){return Ext.factory(writer,Ext.data.Writer,currentWriter,'writer');},create:Ext.emptyFn,read:Ext.emptyFn,update:Ext.emptyFn,destroy:Ext.emptyFn,onDestroy:function(){Ext.destroy(this.getReader(),this.getWriter());},batch:function(options,listeners){var me=this,useBatch=me.getBatchActions(),model=this.getModel(),batch,records;if(options.operations===undefined){options={operations:options,batch:{listeners:listeners}};Ext.Logger.deprecate('Passes old-style signature to Proxy.batch (operations, listeners). Please convert to single options argument syntax.');}
if(options.batch){if(options.batch.isBatch){options.batch.setProxy(me);}else{options.batch.proxy=me;}}else{options.batch={proxy:me,listeners:options.listeners||{}};}
if(!batch){batch=new Ext.data.Batch(options.batch);}
batch.on('complete',Ext.bind(me.onBatchComplete,me,[options],0));Ext.each(me.getBatchOrder().split(','),function(action){records=options.operations[action];if(records){if(useBatch){batch.add(new Ext.data.Operation({action:action,records:records,model:model}));}else{Ext.each(records,function(record){batch.add(new Ext.data.Operation({action:action,records:[record],model:model}));});}}},me);batch.start();return batch;},onBatchComplete:function(batchOptions,batch){var scope=batchOptions.scope||this;if(batch.hasException){if(Ext.isFunction(batchOptions.failure)){Ext.callback(batchOptions.failure,scope,[batch,batchOptions]);}}else if(Ext.isFunction(batchOptions.success)){Ext.callback(batchOptions.success,scope,[batch,batchOptions]);}
if(Ext.isFunction(batchOptions.callback)){Ext.callback(batchOptions.callback,scope,[batch,batchOptions]);}},onClassExtended:function(cls,data){var prototype=this.prototype,defaultConfig=prototype.config,config=data.config||{},key;for(key in defaultConfig){if(key!="control"&&key in data){config[key]=data[key];delete data[key];Ext.Logger.warn(key+' is deprecated as a property directly on the '+this.$className+' prototype. Please put it inside the config object.');}}
data.config=config;}},function(){});Ext.define('Ext.data.proxy.Client',{extend:'Ext.data.proxy.Proxy',alternateClassName:'Ext.proxy.ClientProxy',clear:function(){Ext.Logger.error("The Ext.data.proxy.Client subclass that you are using has not defined a 'clear' function. See src/data/ClientProxy.js for details.");}});Ext.define('Ext.data.proxy.Memory',{extend:'Ext.data.proxy.Client',alias:'proxy.memory',alternateClassName:'Ext.data.MemoryProxy',isMemoryProxy:true,config:{data:[]},finishOperation:function(operation,callback,scope){if(operation){var i=0,recs=operation.getRecords(),len=recs.length;for(i;i<len;i++){recs[i].commit();}
operation.setSuccessful();Ext.callback(callback,scope||this,[operation]);}},create:function(){this.finishOperation.apply(this,arguments);},update:function(){this.finishOperation.apply(this,arguments);},destroy:function(){this.finishOperation.apply(this,arguments);},read:function(operation,callback,scope){var me=this,reader=me.getReader();if(operation.process('read',reader.process(me.getData()))===false){this.fireEvent('exception',this,null,operation);}
Ext.callback(callback,scope||me,[operation]);},clear:Ext.emptyFn});Ext.define('Ext.data.SortTypes',{singleton:true,stripTagsRE:/<\/?[^>]+>/gi,none:function(value){return value;},asText:function(value){return String(value).replace(this.stripTagsRE,"");},asUCText:function(value){return String(value).toUpperCase().replace(this.stripTagsRE,"");},asUCString:function(value){return String(value).toUpperCase();},asDate:function(value){if(!value){return 0;}
if(Ext.isDate(value)){return value.getTime();}
return Date.parse(String(value));},asFloat:function(value){value=parseFloat(String(value).replace(/,/g,""));return isNaN(value)?0:value;},asInt:function(value){value=parseInt(String(value).replace(/,/g,""),10);return isNaN(value)?0:value;}});Ext.define('Ext.data.Types',{singleton:true,requires:['Ext.data.SortTypes'],stripRe:/[\$,%]/g,dashesRe:/-/g,iso8601TestRe:/\d\dT\d\d/,iso8601SplitRe:/[- :T\.Z\+]/},function(){var Types=this,sortTypes=Ext.data.SortTypes;Ext.apply(Types,{AUTO:{convert:function(value){return value;},sortType:sortTypes.none,type:'auto'},STRING:{convert:function(value){return(value===undefined||value===null)?(this.getAllowNull()?null:''):String(value);},sortType:sortTypes.asUCString,type:'string'},INT:{convert:function(value){return(value!==undefined&&value!==null&&value!=='')?((typeof value==='number')?parseInt(value,10):parseInt(String(value).replace(Types.stripRe,''),10)):(this.getAllowNull()?null:0);},sortType:sortTypes.none,type:'int'},FLOAT:{convert:function(value){return(value!==undefined&&value!==null&&value!=='')?((typeof value==='number')?value:parseFloat(String(value).replace(Types.stripRe,''),10)):(this.getAllowNull()?null:0);},sortType:sortTypes.none,type:'float'},BOOL:{convert:function(value){if((value===undefined||value===null||value==='')&&this.getAllowNull()){return null;}
return value===true||value==='true'||value==1;},sortType:sortTypes.none,type:'bool'},DATE:{convert:function(value){var dateFormat=this.getDateFormat(),parsed;if(!value){return null;}
if(Ext.isDate(value)){return value;}
if(dateFormat){if(dateFormat=='timestamp'){return new Date(value*1000);}
if(dateFormat=='time'){return new Date(parseInt(value,10));}
return Ext.Date.parse(value,dateFormat);}
parsed=new Date(Date.parse(value));if(isNaN(parsed)){if(Types.iso8601TestRe.test(value)){parsed=value.split(Types.iso8601SplitRe);parsed=new Date(parsed[0],parsed[1]-1,parsed[2],parsed[3],parsed[4],parsed[5]);}
if(isNaN(parsed)){parsed=new Date(Date.parse(value.replace(this.dashesRe,"/")));if(isNaN(parsed)){Ext.Logger.warn("Cannot parse the passed value ("+value+") into a valid date");}}}
return isNaN(parsed)?null:parsed;},sortType:sortTypes.asDate,type:'date'}});Ext.apply(Types,{BOOLEAN:this.BOOL,INTEGER:this.INT,NUMBER:this.FLOAT});});Ext.define('Ext.data.Field',{requires:['Ext.data.Types','Ext.data.SortTypes'],alias:'data.field',isField:true,config:{name:null,type:'auto',convert:undefined,dateFormat:null,allowNull:true,defaultValue:undefined,mapping:null,sortType:undefined,sortDir:"ASC",allowBlank:true,persist:true,encode:null,decode:null},constructor:function(config){if(Ext.isString(config)){config={name:config};}
this.initConfig(config);},applyType:function(type){var types=Ext.data.Types,autoType=types.AUTO;if(type){if(Ext.isString(type)){return types[type.toUpperCase()]||autoType;}else{return type;}}
return autoType;},updateType:function(newType,oldType){var convert=this.getConvert();if(oldType&&convert===oldType.convert){this.setConvert(newType.convert);}},applySortType:function(sortType){var sortTypes=Ext.data.SortTypes,type=this.getType(),defaultSortType=type.sortType;if(sortType){if(Ext.isString(sortType)){return sortTypes[sortType]||defaultSortType;}else{return sortType;}}
return defaultSortType;},applyConvert:function(convert){var defaultConvert=this.getType().convert;if(convert&&convert!==defaultConvert){this._hasCustomConvert=true;return convert;}else{this._hasCustomConvert=false;return defaultConvert;}},hasCustomConvert:function(){return this._hasCustomConvert;}},function(){Ext.deprecateProperty(this,'useNull','allowNull');});Ext.define('Ext.data.identifier.Simple',{alias:'data.identifier.simple',statics:{AUTO_ID:1},config:{prefix:'ext-record-'},constructor:function(config){this.initConfig(config);},generate:function(record){return this._prefix+this.self.AUTO_ID++;}});Ext.define('Ext.util.HashMap',{mixins:{observable:'Ext.mixin.Observable'},constructor:function(config){this.callParent();this.mixins.observable.constructor.call(this);this.clear(true);},getCount:function(){return this.length;},getData:function(key,value){if(value===undefined){value=key;key=this.getKey(value);}
return[key,value];},getKey:function(o){return o.id;},add:function(key,value){var me=this,data;if(me.containsKey(key)){throw new Error('This key already exists in the HashMap');}
data=this.getData(key,value);key=data[0];value=data[1];me.map[key]=value;++me.length;me.fireEvent('add',me,key,value);return value;},replace:function(key,value){var me=this,map=me.map,old;if(!me.containsKey(key)){me.add(key,value);}
old=map[key];map[key]=value;me.fireEvent('replace',me,key,value,old);return value;},remove:function(o){var key=this.findKey(o);if(key!==undefined){return this.removeByKey(key);}
return false;},removeByKey:function(key){var me=this,value;if(me.containsKey(key)){value=me.map[key];delete me.map[key];--me.length;me.fireEvent('remove',me,key,value);return true;}
return false;},get:function(key){return this.map[key];},clear:function(initial){var me=this;me.map={};me.length=0;if(initial!==true){me.fireEvent('clear',me);}
return me;},containsKey:function(key){return this.map[key]!==undefined;},contains:function(value){return this.containsKey(this.findKey(value));},getKeys:function(){return this.getArray(true);},getValues:function(){return this.getArray(false);},getArray:function(isKey){var arr=[],key,map=this.map;for(key in map){if(map.hasOwnProperty(key)){arr.push(isKey?key:map[key]);}}
return arr;},each:function(fn,scope){var items=Ext.apply({},this.map),key,length=this.length;scope=scope||this;for(key in items){if(items.hasOwnProperty(key)){if(fn.call(scope,key,items[key],length)===false){break;}}}
return this;},clone:function(){var hash=new Ext.util.HashMap(),map=this.map,key;hash.suspendEvents();for(key in map){if(map.hasOwnProperty(key)){hash.add(key,map[key]);}}
hash.resumeEvents();return hash;},findKey:function(value){var key,map=this.map;for(key in map){if(map.hasOwnProperty(key)&&map[key]===value){return key;}}
return undefined;}});Ext.define('Ext.AbstractManager',{requires:['Ext.util.HashMap'],typeName:'type',constructor:function(config){Ext.apply(this,config||{});this.all=Ext.create('Ext.util.HashMap');this.types={};},get:function(id){return this.all.get(id);},register:function(item){this.all.add(item);},unregister:function(item){this.all.remove(item);},registerType:function(type,cls){this.types[type]=cls;cls[this.typeName]=type;},isRegistered:function(type){return this.types[type]!==undefined;},create:function(config,defaultType){var type=config[this.typeName]||config.type||defaultType,Constructor=this.types[type];if(Constructor==undefined){Ext.Error.raise("The '"+type+"' type has not been registered with this manager");}
return new Constructor(config);},onAvailable:function(id,fn,scope){var all=this.all,item;if(all.containsKey(id)){item=all.get(id);fn.call(scope||item,item);}else{all.on('add',function(map,key,item){if(key==id){fn.call(scope||item,item);all.un('add',fn,scope);}});}},each:function(fn,scope){this.all.each(fn,scope||this);},getCount:function(){return this.all.getCount();}});Ext.define('Ext.data.ModelManager',{extend:'Ext.AbstractManager',alternateClassName:['Ext.ModelMgr','Ext.ModelManager'],singleton:true,modelNamespace:null,registerType:function(name,config){var proto=config.prototype,model;if(proto&&proto.isModel){model=config;}else{config={extend:config.extend||'Ext.data.Model',config:config};model=Ext.define(name,config);}
this.types[name]=model;return model;},onModelDefined:Ext.emptyFn,getModel:function(id){var model=id;if(typeof model=='string'){model=this.types[model];if(!model&&this.modelNamespace){model=this.types[this.modelNamespace+'.'+model];}}
return model;},create:function(config,name,id){var con=typeof name=='function'?name:this.types[name||config.name];return new con(config,id);}},function(){Ext.regModel=function(){Ext.Logger.deprecate('Ext.regModel has been deprecated. Models can now be created by '+'extending Ext.data.Model: Ext.define("MyModel", {extend: "Ext.data.Model", fields: []});.');return this.ModelManager.registerType.apply(this.ModelManager,arguments);};});Ext.define('Ext.data.Request',{config:{action:null,params:null,method:'GET',url:null,operation:null,proxy:null,disableCaching:false,headers:{},callbackKey:null,jsonP:null,jsonData:null,xmlData:null,withCredentials:null,callback:null,scope:null,timeout:30000,records:null,directFn:null,args:null},constructor:function(config){this.initConfig(config);}});Ext.define('Ext.data.proxy.Server',{extend:'Ext.data.proxy.Proxy',alias:'proxy.server',alternateClassName:'Ext.data.ServerProxy',requires:['Ext.data.Request'],config:{url:null,pageParam:'page',startParam:'start',limitParam:'limit',groupParam:'group',sortParam:'sort',filterParam:'filter',directionParam:'dir',enablePagingParams:true,simpleSortMode:false,noCache:true,cacheString:"_dc",timeout:30000,api:{create:undefined,read:undefined,update:undefined,destroy:undefined},extraParams:{}},constructor:function(config){config=config||{};if(config.nocache!==undefined){config.noCache=config.nocache;Ext.Logger.warn('nocache configuration on Ext.data.proxy.Server has been deprecated. Please use noCache.');}
this.callParent([config]);},create:function(){return this.doRequest.apply(this,arguments);},read:function(){return this.doRequest.apply(this,arguments);},update:function(){return this.doRequest.apply(this,arguments);},destroy:function(){return this.doRequest.apply(this,arguments);},setExtraParam:function(name,value){this.getExtraParams()[name]=value;},buildRequest:function(operation){var me=this,params=Ext.applyIf(operation.getParams()||{},me.getExtraParams()||{}),request;params=Ext.applyIf(params,me.getParams(operation));request=Ext.create('Ext.data.Request',{params:params,action:operation.getAction(),records:operation.getRecords(),url:operation.getUrl(),operation:operation,proxy:me});request.setUrl(me.buildUrl(request));operation.setRequest(request);return request;},processResponse:function(success,operation,request,response,callback,scope){var me=this,action=operation.getAction(),reader,resultSet;if(success===true){reader=me.getReader();try{resultSet=reader.process(response);}catch(e){operation.setException(e.message);me.fireEvent('exception',this,response,operation);return;}
if(!operation.getModel()){operation.setModel(this.getModel());}
if(operation.process(action,resultSet,request,response)===false){this.fireEvent('exception',this,response,operation);}}else{me.setException(operation,response);me.fireEvent('exception',this,response,operation);}
if(typeof callback=='function'){callback.call(scope||me,operation);}
me.afterRequest(request,success);},setException:function(operation,response){operation.setException({status:response.status,statusText:response.statusText});},applyEncoding:function(value){return Ext.encode(value);},encodeSorters:function(sorters){var min=[],length=sorters.length,i=0;for(;i<length;i++){min[i]={property:sorters[i].getProperty(),direction:sorters[i].getDirection()};}
return this.applyEncoding(min);},encodeFilters:function(filters){var min=[],length=filters.length,i=0;for(;i<length;i++){min[i]={property:filters[i].getProperty(),value:filters[i].getValue()};}
return this.applyEncoding(min);},getParams:function(operation){var me=this,params={},grouper=operation.getGrouper(),sorters=operation.getSorters(),filters=operation.getFilters(),page=operation.getPage(),start=operation.getStart(),limit=operation.getLimit(),simpleSortMode=me.getSimpleSortMode(),pageParam=me.getPageParam(),startParam=me.getStartParam(),limitParam=me.getLimitParam(),groupParam=me.getGroupParam(),sortParam=me.getSortParam(),filterParam=me.getFilterParam(),directionParam=me.getDirectionParam();if(me.getEnablePagingParams()){if(pageParam&&page!==null){params[pageParam]=page;}
if(startParam&&start!==null){params[startParam]=start;}
if(limitParam&&limit!==null){params[limitParam]=limit;}}
if(groupParam&&grouper){params[groupParam]=me.encodeSorters([grouper]);}
if(sortParam&&sorters&&sorters.length>0){if(simpleSortMode){params[sortParam]=sorters[0].getProperty();params[directionParam]=sorters[0].getDirection();}else{params[sortParam]=me.encodeSorters(sorters);}}
if(filterParam&&filters&&filters.length>0){params[filterParam]=me.encodeFilters(filters);}
return params;},buildUrl:function(request){var me=this,url=me.getUrl(request);if(!url){Ext.Logger.error("You are using a ServerProxy but have not supplied it with a url.");}
if(me.getNoCache()){url=Ext.urlAppend(url,Ext.String.format("{0}={1}",me.getCacheString(),Ext.Date.now()));}
return url;},getUrl:function(request){return request?request.getUrl()||this.getApi()[request.getAction()]||this._url:this._url;},doRequest:function(operation,callback,scope){Ext.Logger.error("The doRequest function has not been implemented on your Ext.data.proxy.Server subclass. See src/data/ServerProxy.js for details");},afterRequest:Ext.emptyFn});Ext.define('Ext.data.Connection',{mixins:{observable:'Ext.mixin.Observable'},statics:{requestId:0},config:{url:null,async:true,method:null,username:'',password:'',disableCaching:true,disableCachingParam:'_dc',timeout:30000,extraParams:null,defaultHeaders:null,useDefaultHeader:true,defaultPostHeader:'application/x-www-form-urlencoded; charset=UTF-8',useDefaultXhrHeader:true,defaultXhrHeader:'XMLHttpRequest',autoAbort:false},textAreaRe:/textarea/i,multiPartRe:/multipart\/form-data/i,lineBreakRe:/\r\n/g,constructor:function(config){this.initConfig(config);this.requests={};},request:function(options){options=options||{};var me=this,scope=options.scope||window,username=options.username||me.getUsername(),password=options.password||me.getPassword()||'',async,requestOptions,request,headers,xhr;if(me.fireEvent('beforerequest',me,options)!==false){requestOptions=me.setOptions(options,scope);if(this.isFormUpload(options)===true){this.upload(options.form,requestOptions.url,requestOptions.data,options);return null;}
if(options.autoAbort===true||me.getAutoAbort()){me.abort();}
xhr=this.getXhrInstance();async=options.async!==false?(options.async||me.getAsync()):false;if(username){xhr.open(requestOptions.method,requestOptions.url,async,username,password);}else{xhr.open(requestOptions.method,requestOptions.url,async);}
headers=me.setupHeaders(xhr,options,requestOptions.data,requestOptions.params);request={id:++this.self.requestId,xhr:xhr,headers:headers,options:options,async:async,timeout:setTimeout(function(){request.timedout=true;me.abort(request);},options.timeout||me.getTimeout())};me.requests[request.id]=request;if(async){xhr.onreadystatechange=Ext.Function.bind(me.onStateChange,me,[request]);}
xhr.send(requestOptions.data);if(!async){return this.onComplete(request);}
return request;}else{Ext.callback(options.callback,options.scope,[options,undefined,undefined]);return null;}},upload:function(form,url,params,options){form=Ext.getDom(form);options=options||{};var id=Ext.id(),frame=document.createElement('iframe'),hiddens=[],encoding='multipart/form-data',buf={target:form.target,method:form.method,encoding:form.encoding,enctype:form.enctype,action:form.action},addField=function(name,value){hiddenItem=document.createElement('input');Ext.fly(hiddenItem).set({type:'hidden',value:value,name:name});form.appendChild(hiddenItem);hiddens.push(hiddenItem);},hiddenItem;Ext.fly(frame).set({id:id,name:id,cls:Ext.baseCSSPrefix+'hide-display',src:Ext.SSL_SECURE_URL});document.body.appendChild(frame);if(document.frames){document.frames[id].name=id;}
Ext.fly(form).set({target:id,method:'POST',enctype:encoding,encoding:encoding,action:url||buf.action});if(params){Ext.iterate(Ext.Object.fromQueryString(params),function(name,value){if(Ext.isArray(value)){Ext.each(value,function(v){addField(name,v);});}else{addField(name,value);}});}
Ext.fly(frame).on('load',Ext.Function.bind(this.onUploadComplete,this,[frame,options]),null,{single:true});form.submit();Ext.fly(form).set(buf);Ext.each(hiddens,function(h){Ext.removeNode(h);});},onUploadComplete:function(frame,options){var me=this,response={responseText:'',responseXML:null},doc,firstChild;try{doc=frame.contentWindow.document||frame.contentDocument||window.frames[id].document;if(doc){if(doc.body){if(this.textAreaRe.test((firstChild=doc.body.firstChild||{}).tagName)){response.responseText=firstChild.value;}else{response.responseText=doc.body.innerHTML;}}
response.responseXML=doc.XMLDocument||doc;}}catch(e){}
me.fireEvent('requestcomplete',me,response,options);Ext.callback(options.success,options.scope,[response,options]);Ext.callback(options.callback,options.scope,[options,true,response]);setTimeout(function(){Ext.removeNode(frame);},100);},isFormUpload:function(options){var form=this.getForm(options);if(form){return(options.isUpload||(this.multiPartRe).test(form.getAttribute('enctype')));}
return false;},getForm:function(options){return Ext.getDom(options.form)||null;},setOptions:function(options,scope){var me=this,params=options.params||{},extraParams=me.getExtraParams(),urlParams=options.urlParams,url=options.url||me.getUrl(),jsonData=options.jsonData,method,disableCache,data;if(Ext.isFunction(params)){params=params.call(scope,options);}
if(Ext.isFunction(url)){url=url.call(scope,options);}
url=this.setupUrl(options,url);if(!url){Ext.Logger.error('No URL specified');}
data=options.rawData||options.xmlData||jsonData||null;if(jsonData&&!Ext.isPrimitive(jsonData)){data=Ext.encode(data);}
if(Ext.isObject(params)){params=Ext.Object.toQueryString(params);}
if(Ext.isObject(extraParams)){extraParams=Ext.Object.toQueryString(extraParams);}
params=params+((extraParams)?((params)?'&':'')+extraParams:'');urlParams=Ext.isObject(urlParams)?Ext.Object.toQueryString(urlParams):urlParams;params=this.setupParams(options,params);method=(options.method||me.getMethod()||((params||data)?'POST':'GET')).toUpperCase();this.setupMethod(options,method);disableCache=options.disableCaching!==false?(options.disableCaching||me.getDisableCaching()):false;if(method==='GET'&&disableCache){url=Ext.urlAppend(url,(options.disableCachingParam||me.getDisableCachingParam())+'='+(new Date().getTime()));}
if((method=='GET'||data)&&params){url=Ext.urlAppend(url,params);params=null;}
if(urlParams){url=Ext.urlAppend(url,urlParams);}
return{url:url,method:method,data:data||params||null};},setupUrl:function(options,url){var form=this.getForm(options);if(form){url=url||form.action;}
return url;},setupParams:function(options,params){var form=this.getForm(options),serializedForm;if(form&&!this.isFormUpload(options)){serializedForm=Ext.Element.serializeForm(form);params=params?(params+'&'+serializedForm):serializedForm;}
return params;},setupMethod:function(options,method){if(this.isFormUpload(options)){return'POST';}
return method;},setupHeaders:function(xhr,options,data,params){var me=this,headers=Ext.apply({},options.headers||{},me.getDefaultHeaders()||{}),contentType=me.getDefaultPostHeader(),jsonData=options.jsonData,xmlData=options.xmlData,key,header;if(!headers['Content-Type']&&(data||params)){if(data){if(options.rawData){contentType='text/plain';}else{if(xmlData&&Ext.isDefined(xmlData)){contentType='text/xml';}else if(jsonData&&Ext.isDefined(jsonData)){contentType='application/json';}}}
headers['Content-Type']=contentType;}
if(me.getUseDefaultXhrHeader()&&!headers['X-Requested-With']){headers['X-Requested-With']=me.getDefaultXhrHeader();}
try{for(key in headers){if(headers.hasOwnProperty(key)){header=headers[key];xhr.setRequestHeader(key,header);}}}catch(e){me.fireEvent('exception',key,header);}
if(options.withCredentials){xhr.withCredentials=options.withCredentials;}
return headers;},getXhrInstance:(function(){var options=[function(){return new XMLHttpRequest();},function(){return new ActiveXObject('MSXML2.XMLHTTP.3.0');},function(){return new ActiveXObject('MSXML2.XMLHTTP');},function(){return new ActiveXObject('Microsoft.XMLHTTP');}],i=0,len=options.length,xhr;for(;i<len;++i){try{xhr=options[i];xhr();break;}catch(e){}}
return xhr;})(),isLoading:function(request){if(!(request&&request.xhr)){return false;}
var state=request.xhr.readyState;return!(state===0||state==4);},abort:function(request){var me=this,requests=me.requests,id;if(request&&me.isLoading(request)){request.xhr.onreadystatechange=null;request.xhr.abort();me.clearTimeout(request);if(!request.timedout){request.aborted=true;}
me.onComplete(request);me.cleanup(request);}else if(!request){for(id in requests){if(requests.hasOwnProperty(id)){me.abort(requests[id]);}}}},abortAll:function(){this.abort();},onStateChange:function(request){if(request.xhr.readyState==4){this.clearTimeout(request);this.onComplete(request);this.cleanup(request);}},clearTimeout:function(request){clearTimeout(request.timeout);delete request.timeout;},cleanup:function(request){request.xhr=null;delete request.xhr;},onComplete:function(request){var me=this,options=request.options,result,success,response;try{result=me.parseStatus(request.xhr.status);if(request.timedout){result.success=false;}}catch(e){result={success:false,isException:false};}
success=result.success;if(success){response=me.createResponse(request);me.fireEvent('requestcomplete',me,response,options);Ext.callback(options.success,options.scope,[response,options]);}else{if(result.isException||request.aborted||request.timedout){response=me.createException(request);}else{response=me.createResponse(request);}
me.fireEvent('requestexception',me,response,options);Ext.callback(options.failure,options.scope,[response,options]);}
Ext.callback(options.callback,options.scope,[options,success,response]);delete me.requests[request.id];return response;},parseStatus:function(status){status=status==1223?204:status;var success=(status>=200&&status<300)||status==304||status==0,isException=false;if(!success){switch(status){case 12002:case 12029:case 12030:case 12031:case 12152:case 13030:isException=true;break;}}
return{success:success,isException:isException};},createResponse:function(request){var xhr=request.xhr,headers={},lines,count,line,index,key,response;if(request.timedout||request.aborted){request.success=false;lines=[];}else{lines=xhr.getAllResponseHeaders().replace(this.lineBreakRe,'\n').split('\n');}
count=lines.length;while(count--){line=lines[count];index=line.indexOf(':');if(index>=0){key=line.substr(0,index).toLowerCase();if(line.charAt(index+1)==' '){++index;}
headers[key]=line.substr(index+1);}}
request.xhr=null;delete request.xhr;response={request:request,requestId:request.id,status:xhr.status,statusText:xhr.statusText,getResponseHeader:function(header){return headers[header.toLowerCase()];},getAllResponseHeaders:function(){return headers;},responseText:xhr.responseText,responseXML:xhr.responseXML};xhr=null;return response;},createException:function(request){return{request:request,requestId:request.id,status:request.aborted?-1:0,statusText:request.aborted?'transaction aborted':'communication failure',aborted:request.aborted,timedout:request.timedout};}});Ext.define('Ext.Ajax',{extend:'Ext.data.Connection',singleton:true,autoAbort:false});Ext.define('Ext.data.proxy.Ajax',{extend:'Ext.data.proxy.Server',requires:['Ext.util.MixedCollection','Ext.Ajax'],alias:'proxy.ajax',alternateClassName:['Ext.data.HttpProxy','Ext.data.AjaxProxy'],config:{actionMethods:{create:'POST',read:'GET',update:'POST',destroy:'POST'},headers:{},withCredentials:false},doRequest:function(operation,callback,scope){var writer=this.getWriter(),request=this.buildRequest(operation);request.setConfig({headers:this.getHeaders(),timeout:this.getTimeout(),method:this.getMethod(request),callback:this.createRequestCallback(request,operation,callback,scope),scope:this});if(operation.getWithCredentials()||this.getWithCredentials()){request.setWithCredentials(true);}
request=writer.write(request);Ext.Ajax.request(request.getCurrentConfig());return request;},getMethod:function(request){return this.getActionMethods()[request.getAction()];},createRequestCallback:function(request,operation,callback,scope){var me=this;return function(options,success,response){me.processResponse(success,operation,request,response,callback,scope);};}});Ext.define('Ext.data.association.Association',{alternateClassName:'Ext.data.Association',requires:['Ext.data.ModelManager'],config:{ownerModel:null,ownerName:undefined,associatedModel:null,associatedName:undefined,associationKey:undefined,primaryKey:'id',reader:null,type:null,name:undefined},statics:{create:function(association){if(!association.isAssociation){if(Ext.isString(association)){association={type:association};}
association.type=association.type.toLowerCase();return Ext.factory(association,Ext.data.association.Association,null,'association');}
return association;}},constructor:function(config){this.initConfig(config);},applyName:function(name){if(!name){name=this.getAssociatedName();}
return name;},applyOwnerModel:function(ownerName){var ownerModel=Ext.data.ModelManager.getModel(ownerName);if(ownerModel===undefined){Ext.Logger.error('The configured ownerModel was not valid (you tried '+ownerName+')');}
return ownerModel;},applyOwnerName:function(ownerName){if(!ownerName){ownerName=this.getOwnerModel().modelName;}
ownerName=ownerName.slice(ownerName.lastIndexOf('.')+1);return ownerName;},updateOwnerModel:function(ownerModel,oldOwnerModel){if(oldOwnerModel){this.setOwnerName(ownerModel.modelName);}},applyAssociatedModel:function(associatedName){var associatedModel=Ext.data.ModelManager.types[associatedName];if(associatedModel===undefined){Ext.Logger.error('The configured associatedModel was not valid (you tried '+associatedName+')');}
return associatedModel;},applyAssociatedName:function(associatedName){if(!associatedName){associatedName=this.getAssociatedModel().modelName;}
associatedName=associatedName.slice(associatedName.lastIndexOf('.')+1);return associatedName;},updateAssociatedModel:function(associatedModel,oldAssociatedModel){if(oldAssociatedModel){this.setAssociatedName(associatedModel.modelName);}},applyReader:function(reader){if(reader){if(Ext.isString(reader)){reader={type:reader};}
if(!reader.isReader){Ext.applyIf(reader,{type:'json'});}}
return Ext.factory(reader,Ext.data.Reader,this.getReader(),'reader');},updateReader:function(reader){reader.setModel(this.getAssociatedModel());},onClassExtended:function(cls,data,hooks){var Component=this,defaultConfig=Component.prototype.config,config=data.config||{},key;for(key in defaultConfig){if(key in data){config[key]=data[key];delete data[key];Ext.Logger.deprecate(key+' is deprecated as a property directly on the Association prototype. '+'Please put it inside the config object.');}}
data.config=config;}});Ext.define('Ext.util.Inflector',{singleton:true,plurals:[[(/(quiz)$/i),"$1zes"],[(/^(ox)$/i),"$1en"],[(/([m|l])ouse$/i),"$1ice"],[(/(matr|vert|ind)ix|ex$/i),"$1ices"],[(/(x|ch|ss|sh)$/i),"$1es"],[(/([^aeiouy]|qu)y$/i),"$1ies"],[(/(hive)$/i),"$1s"],[(/(?:([^f])fe|([lr])f)$/i),"$1$2ves"],[(/sis$/i),"ses"],[(/([ti])um$/i),"$1a"],[(/(buffal|tomat|potat)o$/i),"$1oes"],[(/(bu)s$/i),"$1ses"],[(/(alias|status|sex)$/i),"$1es"],[(/(octop|vir)us$/i),"$1i"],[(/(ax|test)is$/i),"$1es"],[(/^person$/),"people"],[(/^man$/),"men"],[(/^(child)$/),"$1ren"],[(/s$/i),"s"],[(/$/),"s"]],singulars:[[(/(quiz)zes$/i),"$1"],[(/(matr)ices$/i),"$1ix"],[(/(vert|ind)ices$/i),"$1ex"],[(/^(ox)en/i),"$1"],[(/(alias|status)es$/i),"$1"],[(/(octop|vir)i$/i),"$1us"],[(/(cris|ax|test)es$/i),"$1is"],[(/(shoe)s$/i),"$1"],[(/(o)es$/i),"$1"],[(/(bus)es$/i),"$1"],[(/([m|l])ice$/i),"$1ouse"],[(/(x|ch|ss|sh)es$/i),"$1"],[(/(m)ovies$/i),"$1ovie"],[(/(s)eries$/i),"$1eries"],[(/([^aeiouy]|qu)ies$/i),"$1y"],[(/([lr])ves$/i),"$1f"],[(/(tive)s$/i),"$1"],[(/(hive)s$/i),"$1"],[(/([^f])ves$/i),"$1fe"],[(/(^analy)ses$/i),"$1sis"],[(/((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$/i),"$1$2sis"],[(/([ti])a$/i),"$1um"],[(/(n)ews$/i),"$1ews"],[(/people$/i),"person"],[(/s$/i),""]],uncountable:["sheep","fish","series","species","money","rice","information","equipment","grass","mud","offspring","deer","means"],singular:function(matcher,replacer){this.singulars.unshift([matcher,replacer]);},plural:function(matcher,replacer){this.plurals.unshift([matcher,replacer]);},clearSingulars:function(){this.singulars=[];},clearPlurals:function(){this.plurals=[];},isTransnumeral:function(word){return Ext.Array.indexOf(this.uncountable,word)!=-1;},pluralize:function(word){if(this.isTransnumeral(word)){return word;}
var plurals=this.plurals,length=plurals.length,tuple,regex,i;for(i=0;i<length;i++){tuple=plurals[i];regex=tuple[0];if(regex==word||(regex.test&&regex.test(word))){return word.replace(regex,tuple[1]);}}
return word;},singularize:function(word){if(this.isTransnumeral(word)){return word;}
var singulars=this.singulars,length=singulars.length,tuple,regex,i;for(i=0;i<length;i++){tuple=singulars[i];regex=tuple[0];if(regex==word||(regex.test&&regex.test(word))){return word.replace(regex,tuple[1]);}}
return word;},classify:function(word){return Ext.String.capitalize(this.singularize(word));},ordinalize:function(number){var parsed=parseInt(number,10),mod10=parsed%10,mod100=parsed%100;if(11<=mod100&&mod100<=13){return number+"th";}else{switch(mod10){case 1:return number+"st";case 2:return number+"nd";case 3:return number+"rd";default:return number+"th";}}}},function(){var irregulars={alumnus:'alumni',cactus:'cacti',focus:'foci',nucleus:'nuclei',radius:'radii',stimulus:'stimuli',ellipsis:'ellipses',paralysis:'paralyses',oasis:'oases',appendix:'appendices',index:'indexes',beau:'beaux',bureau:'bureaux',tableau:'tableaux',woman:'women',child:'children',man:'men',corpus:'corpora',criterion:'criteria',curriculum:'curricula',genus:'genera',memorandum:'memoranda',phenomenon:'phenomena',foot:'feet',goose:'geese',tooth:'teeth',antenna:'antennae',formula:'formulae',nebula:'nebulae',vertebra:'vertebrae',vita:'vitae'},singular;for(singular in irregulars){this.plural(singular,irregulars[singular]);this.singular(irregulars[singular],singular);}});Ext.define('Ext.data.association.HasMany',{extend:'Ext.data.association.Association',alternateClassName:'Ext.data.HasManyAssociation',requires:['Ext.util.Inflector'],alias:'association.hasmany',config:{foreignKey:undefined,store:undefined,storeName:undefined,filterProperty:null,autoLoad:false},constructor:function(config){config=config||{};if(config.storeConfig){Ext.Logger.warn('storeConfig is deprecated on an association. Instead use the store configuration.');config.store=config.storeConfig;delete config.storeConfig;}
this.callParent([config]);},applyName:function(name){if(!name){name=Ext.util.Inflector.pluralize(this.getAssociatedName().toLowerCase());}
return name;},applyStoreName:function(name){if(!name){name=this.getName()+'Store';}
return name;},applyForeignKey:function(foreignKey){if(!foreignKey){var inverse=this.getInverseAssociation();if(inverse){foreignKey=inverse.getForeignKey();}else{foreignKey=this.getOwnerName().toLowerCase()+'_id';}}
return foreignKey;},applyAssociationKey:function(associationKey){if(!associationKey){var associatedName=this.getAssociatedName();associationKey=Ext.util.Inflector.pluralize(associatedName[0].toLowerCase()+associatedName.slice(1));}
return associationKey;},updateForeignKey:function(foreignKey,oldForeignKey){var fields=this.getAssociatedModel().getFields(),field=fields.get(foreignKey);if(!field){field=new Ext.data.Field({name:foreignKey});fields.add(field);fields.isDirty=true;}
if(oldForeignKey){field=fields.get(oldForeignKey);if(field){fields.remove(field);fields.isDirty=true;}}},applyStore:function(storeConfig){var me=this,association=me,associatedModel=me.getAssociatedModel(),storeName=me.getStoreName(),foreignKey=me.getForeignKey(),primaryKey=me.getPrimaryKey(),filterProperty=me.getFilterProperty(),autoLoad=me.getAutoLoad();return function(){var me=this,config,filter,modelDefaults={};if(me[storeName]===undefined){if(filterProperty){filter={property:filterProperty,value:me.get(filterProperty),exactMatch:true};}else{filter={property:foreignKey,value:me.get(primaryKey),exactMatch:true};}
modelDefaults[foreignKey]=me.get(primaryKey);config=Ext.apply({},storeConfig,{model:associatedModel,filters:[filter],remoteFilter:true,modelDefaults:modelDefaults});me[storeName]=Ext.create('Ext.data.Store',config);if(autoLoad){me[storeName].load(function(records,operation){association.updateInverseInstances(me);});}}
return me[storeName];};},updateStore:function(store){this.getOwnerModel().prototype[this.getName()]=store;},read:function(record,reader,associationData){var store=record[this.getName()](),records=reader.read(associationData).getRecords();store.add(records);this.updateInverseInstances(record);},updateInverseInstances:function(record){var store=record[this.getName()](),inverse=this.getInverseAssociation();if(inverse){store.each(function(associatedRecord){associatedRecord[inverse.getInstanceName()]=record;});}},getInverseAssociation:function(){var ownerName=this.getOwnerModel().modelName;return this.getAssociatedModel().associations.findBy(function(assoc){return assoc.getType().toLowerCase()==='belongsto'&&assoc.getAssociatedModel().modelName===ownerName;});}},function(){Ext.deprecateProperty(this,'storeConfig','store');});Ext.define('Ext.data.association.BelongsTo',{extend:'Ext.data.association.Association',alternateClassName:'Ext.data.BelongsToAssociation',alias:'association.belongsto',config:{foreignKey:undefined,getterName:undefined,setterName:undefined,instanceName:undefined},applyForeignKey:function(foreignKey){if(!foreignKey){foreignKey=this.getAssociatedName().toLowerCase()+'_id';}
return foreignKey;},updateForeignKey:function(foreignKey,oldForeignKey){var fields=this.getOwnerModel().getFields(),field=fields.get(foreignKey);if(!field){field=new Ext.data.Field({name:foreignKey});fields.add(field);fields.isDirty=true;}
if(oldForeignKey){field=fields.get(oldForeignKey);if(field){fields.isDirty=true;fields.remove(field);}}},applyInstanceName:function(instanceName){if(!instanceName){instanceName=this.getAssociatedName()+'BelongsToInstance';}
return instanceName;},applyAssociationKey:function(associationKey){if(!associationKey){var associatedName=this.getAssociatedName();associationKey=associatedName[0].toLowerCase()+associatedName.slice(1);}
return associationKey;},applyGetterName:function(getterName){if(!getterName){var associatedName=this.getAssociatedName();getterName='get'+associatedName[0].toUpperCase()+associatedName.slice(1);}
return getterName;},applySetterName:function(setterName){if(!setterName){var associatedName=this.getAssociatedName();setterName='set'+associatedName[0].toUpperCase()+associatedName.slice(1);}
return setterName;},updateGetterName:function(getterName,oldGetterName){var ownerProto=this.getOwnerModel().prototype;if(oldGetterName){delete ownerProto[oldGetterName];}
if(getterName){ownerProto[getterName]=this.createGetter();}},updateSetterName:function(setterName,oldSetterName){var ownerProto=this.getOwnerModel().prototype;if(oldSetterName){delete ownerProto[oldSetterName];}
if(setterName){ownerProto[setterName]=this.createSetter();}},createSetter:function(){var me=this,foreignKey=me.getForeignKey();return function(value,options,scope){var inverse=me.getInverseAssociation();if(value&&value.isModel){value=value.getId();}
this.set(foreignKey,value);if(Ext.isFunction(options)){options={callback:options,scope:scope||this};}
if(inverse){value=Ext.data.Model.cache.get(Ext.data.Model.generateCacheId(inverse.getOwnerModel().modelName,value));if(value){if(inverse.getType().toLowerCase()==='hasmany'){var store=value[inverse.getName()]();store.add(this);}else{value[inverse.getInstanceName()]=this;}}}
if(Ext.isObject(options)){return this.save(options);}};},createGetter:function(){var me=this,associatedModel=me.getAssociatedModel(),foreignKey=me.getForeignKey(),instanceName=me.getInstanceName();return function(options,scope){options=options||{};var model=this,foreignKeyId=model.get(foreignKey),success,instance,args;instance=model[instanceName];if(!instance){instance=Ext.data.Model.cache.get(Ext.data.Model.generateCacheId(associatedModel.modelName,foreignKeyId));if(instance){model[instanceName]=instance;}}
if(options.reload===true||instance===undefined){if(typeof options=='function'){options={callback:options,scope:scope||model};}
success=options.success;options.success=function(rec){model[instanceName]=rec;if(success){success.call(this,arguments);}};associatedModel.load(foreignKeyId,options);}else{args=[instance];scope=scope||model;Ext.callback(options,scope,args);Ext.callback(options.success,scope,args);Ext.callback(options.failure,scope,args);Ext.callback(options.callback,scope,args);return instance;}};},read:function(record,reader,associationData){record[this.getInstanceName()]=reader.read([associationData]).getRecords()[0];},getInverseAssociation:function(){var ownerName=this.getOwnerModel().modelName,foreignKey=this.getForeignKey();return this.getAssociatedModel().associations.findBy(function(assoc){var type=assoc.getType().toLowerCase();return(type==='hasmany'||type==='hasone')&&assoc.getAssociatedModel().modelName===ownerName&&assoc.getForeignKey()===foreignKey;});}});Ext.define('Ext.data.association.HasOne',{extend:'Ext.data.association.Association',alternateClassName:'Ext.data.HasOneAssociation',alias:'association.hasone',config:{foreignKey:undefined,getterName:undefined,setterName:undefined,instanceName:undefined},applyForeignKey:function(foreignKey){if(!foreignKey){var inverse=this.getInverseAssociation();if(inverse){foreignKey=inverse.getForeignKey();}else{foreignKey=this.getOwnerName().toLowerCase()+'_id';}}
return foreignKey;},updateForeignKey:function(foreignKey,oldForeignKey){var fields=this.getAssociatedModel().getFields(),field=fields.get(foreignKey);if(!field){field=new Ext.data.Field({name:foreignKey});fields.add(field);fields.isDirty=true;}
if(oldForeignKey){field=fields.get(oldForeignKey);if(field){fields.remove(field);fields.isDirty=true;}}},applyInstanceName:function(instanceName){if(!instanceName){instanceName=this.getAssociatedName()+'BelongsToInstance';}
return instanceName;},applyAssociationKey:function(associationKey){if(!associationKey){var associatedName=this.getAssociatedName();associationKey=associatedName[0].toLowerCase()+associatedName.slice(1);}
return associationKey;},applyGetterName:function(getterName){if(!getterName){var associatedName=this.getAssociatedName();getterName='get'+associatedName[0].toUpperCase()+associatedName.slice(1);}
return getterName;},applySetterName:function(setterName){if(!setterName){var associatedName=this.getAssociatedName();setterName='set'+associatedName[0].toUpperCase()+associatedName.slice(1);}
return setterName;},updateGetterName:function(getterName,oldGetterName){var ownerProto=this.getOwnerModel().prototype;if(oldGetterName){delete ownerProto[oldGetterName];}
if(getterName){ownerProto[getterName]=this.createGetter();}},updateSetterName:function(setterName,oldSetterName){var ownerProto=this.getOwnerModel().prototype;if(oldSetterName){delete ownerProto[oldSetterName];}
if(setterName){ownerProto[setterName]=this.createSetter();}},createSetter:function(){var me=this,foreignKey=me.getForeignKey(),instanceName=me.getInstanceName(),associatedModel=me.getAssociatedModel();return function(value,options,scope){var Model=Ext.data.Model,record;if(value&&value.isModel){value=value.getId();}
this.set(foreignKey,value);record=Model.cache.get(Model.generateCacheId(associatedModel.modelName,value));if(record){this[instanceName]=record;}
if(Ext.isFunction(options)){options={callback:options,scope:scope||this};}
if(Ext.isObject(options)){return this.save(options);}};},createGetter:function(){var me=this,associatedModel=me.getAssociatedModel(),foreignKey=me.getForeignKey(),instanceName=me.getInstanceName();return function(options,scope){options=options||{};var model=this,foreignKeyId=model.get(foreignKey),success,instance,args;if(options.reload===true||model[instanceName]===undefined){if(typeof options=='function'){options={callback:options,scope:scope||model};}
success=options.success;options.success=function(rec){model[instanceName]=rec;if(success){success.call(this,arguments);}};associatedModel.load(foreignKeyId,options);}else{instance=model[instanceName];args=[instance];scope=scope||model;Ext.callback(options,scope,args);Ext.callback(options.success,scope,args);Ext.callback(options.failure,scope,args);Ext.callback(options.callback,scope,args);return instance;}};},read:function(record,reader,associationData){var inverse=this.getInverseAssociation(),newRecord=reader.read([associationData]).getRecords()[0];record[this.getInstanceName()]=newRecord;if(inverse){newRecord[inverse.getInstanceName()]=record;}},getInverseAssociation:function(){var ownerName=this.getOwnerModel().modelName;return this.getAssociatedModel().associations.findBy(function(assoc){return assoc.getType().toLowerCase()==='belongsto'&&assoc.getAssociatedModel().modelName===ownerName;});}});Ext.define('Ext.data.Error',{config:{field:null,message:''},constructor:function(config){this.initConfig(config);}});Ext.define('Ext.data.Errors',{extend:'Ext.util.Collection',requires:'Ext.data.Error',isValid:function(){return this.length===0;},getByField:function(fieldName){var errors=[],error,i;for(i=0;i<this.length;i++){error=this.items[i];if(error.getField()==fieldName){errors.push(error);}}
return errors;},add:function(){var obj=arguments.length==1?arguments[0]:arguments[1];if(!(obj instanceof Ext.data.Error)){obj=Ext.create('Ext.data.Error',{field:obj.field||obj.name,message:obj.error||obj.message});}
return this.callParent([obj]);}});Ext.define('Ext.data.Model',{alternateClassName:'Ext.data.Record',mixins:{observable:'Ext.mixin.Observable'},isModel:true,requires:['Ext.util.Collection','Ext.data.Field','Ext.data.identifier.Simple','Ext.data.ModelManager','Ext.data.proxy.Ajax','Ext.data.association.HasMany','Ext.data.association.BelongsTo','Ext.data.association.HasOne','Ext.data.Errors'],config:{idProperty:'id',data:null,fields:undefined,validations:null,associations:null,hasMany:null,hasOne:null,belongsTo:null,proxy:null,identifier:{type:'simple'},clientIdProperty:'clientId',isErased:false},staticConfigs:['idProperty','fields','validations','associations','hasMany','hasOne','belongsTo','clientIdProperty','identifier','proxy'],statics:{EDIT:'edit',REJECT:'reject',COMMIT:'commit',generateProxyMethod:function(name){return function(){var prototype=this.prototype;return prototype[name].apply(prototype,arguments);};},generateCacheId:function(record,id){var modelName;if(record&&record.isModel){modelName=record.modelName;if(id===undefined){id=record.getId();}}else{modelName=record;}
return modelName.replace(/\./g,'-').toLowerCase()+'-'+id;}},inheritableStatics:{load:function(id,config,scope){var proxy=this.getProxy(),idProperty=this.getIdProperty(),record=null,params={},callback,operation;scope=scope||(config&&config.scope)||this;if(Ext.isFunction(config)){config={callback:config,scope:scope};}
params[idProperty]=id;config=Ext.apply({},config);config=Ext.applyIf(config,{action:'read',params:params,model:this});operation=Ext.create('Ext.data.Operation',config);if(!proxy){Ext.Logger.error('You are trying to load a model that doesn\'t have a Proxy specified');}
callback=function(operation){if(operation.wasSuccessful()){record=operation.getRecords()[0];Ext.callback(config.success,scope,[record,operation]);}else{Ext.callback(config.failure,scope,[record,operation]);}
Ext.callback(config.callback,scope,[record,operation]);};proxy.read(operation,callback,this);}},editing:false,dirty:false,phantom:false,constructor:function(data,id,raw,convertedData){var me=this,cached=null,idProperty=this.getIdProperty();me.modified={};me.raw=raw||data||{};me.stores=[];data=data||convertedData||{};if(id||id===0){data[idProperty]=me.internalId=id;}
id=me.data[idProperty];if(id||id===0){cached=Ext.data.Model.cache.get(Ext.data.Model.generateCacheId(this,id));if(cached){return cached.mergeData(convertedData||data||{});}}
if(convertedData){me.setConvertedData(data);}else{me.setData(data);}
id=me.data[idProperty];if(!id&&id!==0){me.data[idProperty]=me.internalId=me.id=me.getIdentifier().generate(me);me.phantom=true;if(this.associations.length){this.handleInlineAssociationData(data);}}else{me.id=me.getIdentifier().generate(me);}
Ext.data.Model.cache.add(me);if(this.init&&typeof this.init=='function'){this.init();}},mergeData:function(rawData){var me=this,fields=me.getFields().items,ln=fields.length,data=me.data,i,field,fieldName,value,convert;for(i=0;i<ln;i++){field=fields[i];fieldName=field.getName();convert=field.getConvert();value=rawData[fieldName];if(value!==undefined&&!me.isModified(fieldName)){if(convert){value=convert.call(field,value,me);}
data[fieldName]=value;}}
return this;},setData:function(rawData){var fields=this.fields.items,ln=fields.length,isArray=Ext.isArray(rawData),data=this._data=this.data={},i,field,name,value,convert,id;if(!rawData){return this;}
for(i=0;i<ln;i++){field=fields[i];name=field.getName();convert=field.getConvert();if(isArray){value=rawData[i];}
else{value=rawData[name];if(typeof value=='undefined'){value=field.getDefaultValue();}}
if(convert){value=convert.call(field,value,this);}
data[name]=value;}
id=this.getId();if(this.associations.length&&(id||id===0)){this.handleInlineAssociationData(rawData);}
return this;},handleInlineAssociationData:function(data){var associations=this.associations.items,ln=associations.length,i,association,associationData,reader,proxy,associationKey;for(i=0;i<ln;i++){association=associations[i];associationKey=association.getAssociationKey();associationData=data[associationKey];if(associationData){reader=association.getReader();if(!reader){proxy=association.getAssociatedModel().getProxy();if(proxy){reader=proxy.getReader();}else{reader=new Ext.data.JsonReader({model:association.getAssociatedModel()});}}
association.read(this,reader,associationData);}}},setId:function(id){var currentId=this.getId();this.set(this.getIdProperty(),id);this.internalId=id;Ext.data.Model.cache.replace(currentId,this);},getId:function(){return this.get(this.getIdProperty());},setConvertedData:function(data){this._data=this.data=data;return this;},get:function(fieldName){return this.data[fieldName];},set:function(fieldName,value){var me=this,fieldMap=me.fields.map,modified=me.modified,notEditing=!me.editing,associations=me.associations.items,modifiedCount=0,modifiedFieldNames=[],field,key,i,currentValue,ln,convert;if(arguments.length==1){for(key in fieldName){if(fieldName.hasOwnProperty(key)){field=fieldMap[key];if(field&&field.hasCustomConvert()){modifiedFieldNames.push(key);continue;}
if(!modifiedCount&&notEditing){me.beginEdit();}
++modifiedCount;me.set(key,fieldName[key]);}}
ln=modifiedFieldNames.length;if(ln){if(!modifiedCount&&notEditing){me.beginEdit();}
modifiedCount+=ln;for(i=0;i<ln;i++){field=modifiedFieldNames[i];me.set(field,fieldName[field]);}}
if(notEditing&&modifiedCount){me.endEdit(false,modifiedFieldNames);}}else{field=fieldMap[fieldName];convert=field&&field.getConvert();if(convert){value=convert.call(field,value,me);}
currentValue=me.data[fieldName];me.data[fieldName]=value;if(field&&!me.isEqual(currentValue,value)){if(modified.hasOwnProperty(fieldName)){if(me.isEqual(modified[fieldName],value)){delete modified[fieldName];me.dirty=false;for(key in modified){if(modified.hasOwnProperty(key)){me.dirty=true;break;}}}}else{me.dirty=true;modified[fieldName]=currentValue;}}
if(notEditing){me.afterEdit([fieldName],modified);}}},isEqual:function(a,b){if(Ext.isDate(a)&&Ext.isDate(b)){return a.getTime()===b.getTime();}
return a===b;},beginEdit:function(){var me=this;if(!me.editing){me.editing=true;me.dirtySave=me.dirty;me.dataSave=Ext.apply({},me.data);me.modifiedSave=Ext.apply({},me.modified);}},cancelEdit:function(){var me=this;if(me.editing){me.editing=false;me.modified=me.modifiedSave;me.data=me.dataSave;me.dirty=me.dirtySave;delete me.modifiedSave;delete me.dataSave;delete me.dirtySave;}},endEdit:function(silent,modifiedFieldNames){var me=this;if(me.editing){me.editing=false;if(silent!==true&&(me.changedWhileEditing())){me.afterEdit(modifiedFieldNames||Ext.Object.getKeys(this.modified),this.modified);}
delete me.modifiedSave;delete me.dataSave;delete me.dirtySave;}},changedWhileEditing:function(){var me=this,saved=me.dataSave,data=me.data,key;for(key in data){if(data.hasOwnProperty(key)){if(!me.isEqual(data[key],saved[key])){return true;}}}
return false;},getChanges:function(){var modified=this.modified,changes={},field;for(field in modified){if(modified.hasOwnProperty(field)){changes[field]=this.get(field);}}
return changes;},isModified:function(fieldName){return this.modified.hasOwnProperty(fieldName);},save:function(options,scope){var me=this,action=me.phantom?'create':'update',proxy=me.getProxy(),operation,callback;if(!proxy){Ext.Logger.error('You are trying to save a model instance that doesn\'t have a Proxy specified');}
options=options||{};scope=scope||me;if(Ext.isFunction(options)){options={callback:options,scope:scope};}
Ext.applyIf(options,{records:[me],action:action,model:me.self});operation=Ext.create('Ext.data.Operation',options);callback=function(operation){if(operation.wasSuccessful()){Ext.callback(options.success,scope,[me,operation]);}else{Ext.callback(options.failure,scope,[me,operation]);}
Ext.callback(options.callback,scope,[me,operation]);};proxy[action](operation,callback,me);return me;},erase:function(options,scope){var me=this,proxy=this.getProxy(),operation,callback;if(!proxy){Ext.Logger.error('You are trying to erase a model instance that doesn\'t have a Proxy specified');}
options=options||{};scope=scope||me;if(Ext.isFunction(options)){options={callback:options,scope:scope};}
Ext.applyIf(options,{records:[me],action:'destroy',model:this.self});operation=Ext.create('Ext.data.Operation',options);callback=function(operation){if(operation.wasSuccessful()){Ext.callback(options.success,scope,[me,operation]);}else{Ext.callback(options.failure,scope,[me,operation]);}
Ext.callback(options.callback,scope,[me,operation]);};proxy.destroy(operation,callback,me);return me;},reject:function(silent){var me=this,modified=me.modified,field;for(field in modified){if(modified.hasOwnProperty(field)){if(typeof modified[field]!="function"){me.data[field]=modified[field];}}}
me.dirty=false;me.editing=false;me.modified={};if(silent!==true){me.afterReject();}},commit:function(silent){var me=this,modified=this.modified;me.phantom=me.dirty=me.editing=false;me.modified={};if(silent!==true){me.afterCommit(modified);}},afterEdit:function(modifiedFieldNames,modified){this.notifyStores('afterEdit',modifiedFieldNames,modified);},afterReject:function(){this.notifyStores("afterReject");},afterCommit:function(modified){this.notifyStores('afterCommit',Ext.Object.getKeys(modified||{}),modified);},notifyStores:function(fn){var args=Ext.Array.clone(arguments),stores=this.stores,ln=stores.length,i,store;args[0]=this;for(i=0;i<ln;++i){store=stores[i];if(store!==undefined&&typeof store[fn]=="function"){store[fn].apply(store,args);}}},copy:function(newId){var me=this,idProperty=me.getIdProperty(),raw=Ext.apply({},me.raw),data=Ext.apply({},me.data);delete raw[idProperty];delete data[idProperty];return new me.self(null,newId,raw,data);},getData:function(includeAssociated){var data=this.data;if(includeAssociated===true){Ext.apply(data,this.getAssociatedData());}
return data;},getAssociatedData:function(){return this.prepareAssociatedData(this,[],null);},prepareAssociatedData:function(record,ids,associationType){var associations=record.associations.items,associationCount=associations.length,associationData={},associatedStore,associationName,associatedRecords,associatedRecord,associatedRecordCount,association,id,i,j,type,allow;for(i=0;i<associationCount;i++){association=associations[i];associationName=association.getName();type=association.getType();allow=true;if(associationType){allow=type==associationType;}
if(allow&&type.toLowerCase()=='hasmany'){associatedStore=record[association.getStoreName()];associationData[associationName]=[];if(associatedStore&&associatedStore.getCount()>0){associatedRecords=associatedStore.data.items;associatedRecordCount=associatedRecords.length;for(j=0;j<associatedRecordCount;j++){associatedRecord=associatedRecords[j];id=associatedRecord.id;if(Ext.Array.indexOf(ids,id)==-1){ids.push(id);associationData[associationName][j]=associatedRecord.getData();Ext.apply(associationData[associationName][j],this.prepareAssociatedData(associatedRecord,ids,associationType));}}}}else if(allow&&(type.toLowerCase()=='belongsto'||type.toLowerCase()=='hasone')){associatedRecord=record[association.getInstanceName()];if(associatedRecord!==undefined){id=associatedRecord.id;if(Ext.Array.indexOf(ids,id)===-1){ids.push(id);associationData[associationName]=associatedRecord.getData();Ext.apply(associationData[associationName],this.prepareAssociatedData(associatedRecord,ids,associationType));}}}}
return associationData;},join:function(store){Ext.Array.include(this.stores,store);},unjoin:function(store){Ext.Array.remove(this.stores,store);},setDirty:function(){var me=this,name;me.dirty=true;me.fields.each(function(field){if(field.getPersist()){name=field.getName();me.modified[name]=me.get(name);}});},validate:function(){var errors=Ext.create('Ext.data.Errors'),validations=this.getValidations().items,validators=Ext.data.Validations,length,validation,field,valid,type,i;if(validations){length=validations.length;for(i=0;i<length;i++){validation=validations[i];field=validation.field||validation.name;type=validation.type;valid=validators[type](validation,this.get(field));if(!valid){errors.add(Ext.create('Ext.data.Error',{field:field,message:validation.message||validators.getMessage(type)}));}}}
return errors;},isValid:function(){return this.validate().isValid();},toUrl:function(){var pieces=this.$className.split('.'),name=pieces[pieces.length-1].toLowerCase();return name+'/'+this.getId();},destroy:function(){var me=this;me.notifyStores('afterErase',me);Ext.data.Model.cache.remove(me);me.raw=me.stores=me.modified=null;me.callParent(arguments);},markDirty:function(){if(Ext.isDefined(Ext.Logger)){Ext.Logger.deprecate('Ext.data.Model: markDirty has been deprecated. Use setDirty instead.');}
return this.setDirty.apply(this,arguments);},applyProxy:function(proxy,currentProxy){return Ext.factory(proxy,Ext.data.Proxy,currentProxy,'proxy');},updateProxy:function(proxy){if(proxy){proxy.setModel(this.self);}},applyAssociations:function(associations){if(associations){this.addAssociations(associations,'hasMany');}},applyBelongsTo:function(belongsTo){if(belongsTo){this.addAssociations(belongsTo,'belongsTo');}},applyHasMany:function(hasMany){if(hasMany){this.addAssociations(hasMany,'hasMany');}},applyHasOne:function(hasOne){if(hasOne){this.addAssociations(hasOne,'hasOne');}},addAssociations:function(associations,defaultType){var ln,i,association,name=this.self.modelName,associationsCollection=this.self.associations,onCreatedFn;associations=Ext.Array.from(associations);for(i=0,ln=associations.length;i<ln;i++){association=associations[i];if(!Ext.isObject(association)){association={model:association};}
Ext.applyIf(association,{type:defaultType,ownerModel:name,associatedModel:association.model});delete association.model;onCreatedFn=Ext.Function.bind(function(associationName){associationsCollection.add(Ext.data.association.Association.create(this));},association);Ext.ClassManager.onCreated(onCreatedFn,this,(typeof association.associatedModel==='string')?association.associatedModel:Ext.getClassName(association.associatedModel));}},applyValidations:function(validations){if(validations){if(!Ext.isArray(validations)){validations=[validations];}
this.addValidations(validations);}},addValidations:function(validations){this.self.validations.addAll(validations);},applyFields:function(fields){var superFields=this.superclass.fields;if(superFields){fields=superFields.items.concat(fields||[]);}
return fields||[];},updateFields:function(fields){var ln=fields.length,me=this,prototype=me.self.prototype,idProperty=this.getIdProperty(),idField,fieldsCollection,field,i;fieldsCollection=me._fields=me.fields=new Ext.util.Collection(prototype.getFieldName);for(i=0;i<ln;i++){field=fields[i];if(!field.isField){field=new Ext.data.Field(fields[i]);}
fieldsCollection.add(field);}
idField=fieldsCollection.get(idProperty);if(!idField){fieldsCollection.add(new Ext.data.Field(idProperty));}else{idField.setType('auto');}
fieldsCollection.addSorter(prototype.sortConvertFields);},applyIdentifier:function(identifier){if(typeof identifier==='string'){identifier={type:identifier};}
return Ext.factory(identifier,Ext.data.identifier.Simple,this.getIdentifier(),'data.identifier');},getFieldName:function(field){return field.getName();},sortConvertFields:function(field1,field2){var f1SpecialConvert=field1.hasCustomConvert(),f2SpecialConvert=field2.hasCustomConvert();if(f1SpecialConvert&&!f2SpecialConvert){return 1;}
if(!f1SpecialConvert&&f2SpecialConvert){return-1;}
return 0;},onClassExtended:function(cls,data,hooks){var onBeforeClassCreated=hooks.onBeforeCreated,Model=this,prototype=Model.prototype,configNameCache=Ext.Class.configNameCache,staticConfigs=prototype.staticConfigs.concat(data.staticConfigs||[]),defaultConfig=prototype.config,config=data.config||{},key;if(data.idgen||config.idgen){config.identifier=data.idgen||config.idgen;Ext.Logger.deprecate('idgen is deprecated as a property. Please put it inside the config object'+' under the new "identifier" configuration');}
for(key in defaultConfig){if(key in data){config[key]=data[key];delete data[key];Ext.Logger.deprecate(key+' is deprecated as a property directly on the Model prototype. '+'Please put it inside the config object.');}}
data.config=config;hooks.onBeforeCreated=function(cls,data){var dependencies=[],prototype=cls.prototype,statics={},config=prototype.config,staticConfigsLn=staticConfigs.length,copyMethods=['set','get'],copyMethodsLn=copyMethods.length,associations=config.associations||[],name=Ext.getClassName(cls),key,methodName,i,j,ln;for(i=0;i<staticConfigsLn;i++){key=staticConfigs[i];for(j=0;j<copyMethodsLn;j++){methodName=configNameCache[key][copyMethods[j]];if(methodName in prototype){statics[methodName]=Model.generateProxyMethod(methodName);}}}
cls.addStatics(statics);cls.modelName=name;prototype.modelName=name;if(config.belongsTo){dependencies.push('association.belongsto');}
if(config.hasMany){dependencies.push('association.hasmany');}
if(config.hasOne){dependencies.push('association.hasone');}
for(i=0,ln=associations.length;i<ln;++i){dependencies.push('association.'+associations[i].type.toLowerCase());}
if(config.proxy){if(typeof config.proxy==='string'){dependencies.push('proxy.'+config.proxy);}
else if(typeof config.proxy.type==='string'){dependencies.push('proxy.'+config.proxy.type);}}
if(config.validations){dependencies.push('Ext.data.Validations');}
Ext.require(dependencies,function(){Ext.Function.interceptBefore(hooks,'onCreated',function(){Ext.data.ModelManager.registerType(name,cls);var superCls=cls.prototype.superclass;cls.prototype.associations=cls.associations=cls.prototype._associations=(superCls&&superCls.associations)?superCls.associations.clone():new Ext.util.Collection(function(association){return association.getName();});cls.prototype.validations=cls.validations=cls.prototype._validations=(superCls&&superCls.validations)?superCls.validations.clone():new Ext.util.Collection(function(validation){return validation.field||validation.name;});cls.prototype=Ext.Object.chain(cls.prototype);cls.prototype.initConfig.call(cls.prototype,config);delete cls.prototype.initConfig;});onBeforeClassCreated.call(Model,cls,data,hooks);});};}},function(){this.cache=new Ext.util.Collection(this.generateCacheId);});Ext.define('Ext.data.StoreManager',{extend:'Ext.util.Collection',alternateClassName:['Ext.StoreMgr','Ext.data.StoreMgr','Ext.StoreManager'],singleton:true,uses:['Ext.data.ArrayStore'],register:function(){for(var i=0,s;(s=arguments[i]);i++){this.add(s);}},unregister:function(){for(var i=0,s;(s=arguments[i]);i++){this.remove(this.lookup(s));}},lookup:function(store){if(Ext.isArray(store)){var fields=['field1'],expand=!Ext.isArray(store[0]),data=store,i,len;if(expand){data=[];for(i=0,len=store.length;i<len;++i){data.push([store[i]]);}}else{for(i=2,len=store[0].length;i<=len;++i){fields.push('field'+i);}}
return Ext.create('Ext.data.ArrayStore',{data:data,fields:fields,autoDestroy:true,autoCreated:true,expanded:expand});}
if(Ext.isString(store)){return this.get(store);}else{if(store instanceof Ext.data.Store){return store;}else{return Ext.factory(store,Ext.data.Store,null,'store');}}},getKey:function(o){return o.getStoreId();}},function(){Ext.regStore=function(name,config){var store;if(Ext.isObject(name)){config=name;}else{if(config instanceof Ext.data.Store){config.setStoreId(name);}else{config.storeId=name;}}
if(config instanceof Ext.data.Store){store=config;}else{store=Ext.create('Ext.data.Store',config);}
return Ext.data.StoreManager.register(store);};Ext.getStore=function(name){return Ext.data.StoreManager.lookup(name);};});Ext.define('Ext.util.Grouper',{extend:'Ext.util.Sorter',isGrouper:true,config:{groupFn:null,sortProperty:null,sorterFn:function(item1,item2){var property=this.getSortProperty(),groupFn,group1,group2,modifier;groupFn=this.getGroupFn();group1=groupFn.call(this,item1);group2=groupFn.call(this,item2);if(property){if(group1!==group2){return this.defaultSortFn.call(this,item1,item2);}else{return 0;}}
return(group1>group2)?1:((group1<group2)?-1:0);}},defaultSortFn:function(item1,item2){var me=this,transform=me._transform,root=me._root,value1,value2,property=me._sortProperty;if(root!==null){item1=item1[root];item2=item2[root];}
value1=item1[property];value2=item2[property];if(transform){value1=transform(value1);value2=transform(value2);}
return value1>value2?1:(value1<value2?-1:0);},updateProperty:function(property){this.setGroupFn(this.standardGroupFn);},standardGroupFn:function(item){var root=this.getRoot(),property=this.getProperty(),data=item;if(root){data=item[root];}
return data[property];},getGroupString:function(item){var group=this.getGroupFn().call(this,item);return typeof group!='undefined'?group.toString():'';}});Ext.define('Ext.data.Store',{alias:'store.store',extend:'Ext.Evented',requires:['Ext.util.Collection','Ext.data.Operation','Ext.data.proxy.Memory','Ext.data.Model','Ext.data.StoreManager','Ext.util.Grouper'],statics:{create:function(store){if(!store.isStore){if(!store.type){store.type='store';}
store=Ext.createByAlias('store.'+store.type,store);}
return store;}},isStore:true,config:{storeId:undefined,data:null,autoLoad:null,autoSync:false,model:undefined,proxy:undefined,fields:null,remoteSort:false,remoteFilter:false,remoteGroup:false,filters:null,sorters:null,grouper:null,groupField:null,groupDir:null,getGroupString:null,pageSize:25,totalCount:null,clearOnPageLoad:true,modelDefaults:{},autoDestroy:false,syncRemovedRecords:true},currentPage:1,constructor:function(config){config=config||{};this.data=this._data=this.createDataCollection();this.data.setSortRoot('data');this.data.setFilterRoot('data');this.removed=[];if(config.id&&!config.storeId){config.storeId=config.id;delete config.id;}
if(config.hasOwnProperty('sortOnLoad')){Ext.Logger.deprecate('[Ext.data.Store] sortOnLoad is always activated in Sencha Touch 2 so your Store is always fully '+'sorted after loading. The only expection is if you are using remoteSort and change sorting after '+'the Store as loaded, in which case you need to call store.load() to fetch the sorted data from the server.');}
if(config.hasOwnProperty('filterOnLoad')){Ext.Logger.deprecate('[Ext.data.Store] filterOnLoad is always activated in Sencha Touch 2 so your Store is always fully '+'sorted after loading. The only expection is if you are using remoteFilter and change filtering after '+'the Store as loaded, in which case you need to call store.load() to fetch the filtered data from the server.');}
if(config.hasOwnProperty('sortOnFilter')){Ext.Logger.deprecate('[Ext.data.Store] sortOnFilter is deprecated and is always effectively true when sorting and filtering locally');}
this.initConfig(config);},createDataCollection:function(){return new Ext.util.Collection(function(record){return record.getId();});},applyStoreId:function(storeId){if(storeId===undefined||storeId===null){storeId=this.getUniqueId();}
return storeId;},updateStoreId:function(storeId,oldStoreId){if(oldStoreId){Ext.data.StoreManager.unregister(this);}
if(storeId){Ext.data.StoreManager.register(this);}},applyModel:function(model){if(typeof model=='string'){var registeredModel=Ext.data.ModelManager.getModel(model);if(!registeredModel){Ext.Logger.error('Model with name "'+model+'" does not exist.');}
model=registeredModel;}
if(model&&!model.prototype.isModel&&Ext.isObject(model)){model=Ext.data.ModelManager.registerType(model.storeId||model.id||Ext.id(),model);}
if(!model){var fields=this.getFields(),data=this.config.data;if(!fields&&data&&data.length){fields=Ext.Object.getKeys(data[0]);}
if(fields){model=Ext.define('Ext.data.Store.ImplicitModel-'+(this.getStoreId()||Ext.id()),{extend:'Ext.data.Model',config:{fields:fields,proxy:this.getProxy()}});this.implicitModel=true;}}
if(!model&&this.getProxy()){model=this.getProxy().getModel();}
if(!model){Ext.Logger.warn('Unless you define your model through metadata, a store needs to have a model defined on either itself or on its proxy');}
return model;},updateModel:function(model){var proxy=this.getProxy();if(proxy&&!proxy.getModel()){proxy.setModel(model);}},applyProxy:function(proxy,currentProxy){proxy=Ext.factory(proxy,Ext.data.Proxy,currentProxy,'proxy');if(!proxy&&this.getModel()){proxy=this.getModel().getProxy();if(!proxy){proxy=new Ext.data.proxy.Memory({model:this.getModel()});}}
return proxy;},updateProxy:function(proxy){if(proxy){if(!proxy.getModel()){proxy.setModel(this.getModel());}
proxy.on('metachange',this.onMetaChange,this);}},applyData:function(data){var me=this,proxy;if(data){proxy=me.getProxy();if(proxy instanceof Ext.data.proxy.Memory){proxy.setData(data);me.load();}else{me.removeAll(true);me.fireEvent('clear',me);me.suspendEvents();me.add(data);me.resumeEvents();me.dataLoaded=true;}}else{me.removeAll(true);me.fireEvent('clear',me);}
me.fireEvent('refresh',me,me.data);},clearData:function(){this.setData(null);},addData:function(data){var reader=this.getProxy().getReader(),resultSet=reader.read(data),records=resultSet.getRecords();this.add(records);},updateAutoLoad:function(autoLoad){var proxy=this.getProxy();if(autoLoad&&(proxy&&!proxy.isMemoryProxy)){this.load(Ext.isObject(autoLoad)?autoLoad:null);}},isAutoLoading:function(){var proxy=this.getProxy();return(this.getAutoLoad()||(proxy&&proxy.isMemoryProxy)||this.dataLoaded);},updateGroupField:function(groupField){var grouper=this.getGrouper();if(groupField){if(!grouper){this.setGrouper({property:groupField,direction:this.getGroupDir()||'ASC'});}else{grouper.setProperty(groupField);}}else if(grouper){this.setGrouper(null);}},updateGroupDir:function(groupDir){var grouper=this.getGrouper();if(grouper){grouper.setDirection(groupDir);}},applyGetGroupString:function(getGroupStringFn){var grouper=this.getGrouper();if(getGroupStringFn){Ext.Logger.warn('Specifying getGroupString on a store has been deprecated. Please use grouper: {groupFn: yourFunction}');if(grouper){grouper.setGroupFn(getGroupStringFn);}else{this.setGrouper({groupFn:getGroupStringFn});}}else if(grouper){this.setGrouper(null);}},applyGrouper:function(grouper){if(typeof grouper=='string'){grouper={property:grouper};}
else if(typeof grouper=='function'){grouper={groupFn:grouper};}
grouper=Ext.factory(grouper,Ext.util.Grouper,this.getGrouper());return grouper;},updateGrouper:function(grouper,oldGrouper){var data=this.data;if(oldGrouper){data.removeSorter(oldGrouper);if(!grouper){data.getSorters().removeSorter('isGrouper');}}
if(grouper){data.insertSorter(0,grouper);if(!oldGrouper){data.getSorters().addSorter({direction:'DESC',property:'isGrouper',transform:function(value){return(value===true)?1:-1;}});}}},isGrouped:function(){return!!this.getGrouped();},updateSorters:function(sorters){var grouper=this.getGrouper(),data=this.data,autoSort=data.getAutoSort();data.setAutoSort(false);data.setSorters(sorters);if(grouper){data.insertSorter(0,grouper);}
this.updateSortTypes();data.setAutoSort(autoSort);},updateSortTypes:function(){var model=this.getModel(),fields=model&&model.getFields(),data=this.data;if(fields){data.getSorters().each(function(sorter){var property=sorter.getProperty(),field;if(!sorter.isGrouper&&property&&!sorter.getTransform()){field=fields.get(property);if(field){sorter.setTransform(field.getSortType());}}});}},updateFilters:function(filters){this.data.setFilters(filters);},add:function(records){if(!Ext.isArray(records)){records=Array.prototype.slice.apply(arguments);}
return this.insert(this.data.length,records);},insert:function(index,records){if(!Ext.isArray(records)){records=Array.prototype.slice.call(arguments,1);}
var me=this,sync=false,ln=records.length,Model=this.getModel(),modelDefaults=me.getModelDefaults(),i,record,added=false;records=records.slice();for(i=0;i<ln;i++){record=records[i];if(!record.isModel){record=new Model(record);}
else if(this.removed.indexOf(record)!=-1){Ext.Array.remove(this.removed,record);}
record.set(modelDefaults);records[i]=record;record.join(me);sync=sync||(record.phantom===true);}
if(ln===1){added=this.data.insert(index,records[0]);if(added){added=[added];}}else{added=this.data.insertAll(index,records);}
if(added){me.fireEvent('addrecords',me,added);}
if(me.getAutoSync()&&sync){me.sync();}
return records;},remove:function(records){if(records.isModel){records=[records];}
var me=this,sync=false,i=0,autoSync=this.getAutoSync(),ln=records.length,indices=[],removed=[],isPhantom,items=me.data.items,record,index,j;for(;i<ln;i++){record=records[i];if(me.data.contains(record)){isPhantom=(record.phantom===true);index=items.indexOf(record);if(index!==-1){removed.push(record);indices.push(index);}
if(!isPhantom&&me.getSyncRemovedRecords()){me.removed.push(record);}
record.unjoin(me);me.data.remove(record);sync=sync||!isPhantom;}}
me.fireEvent('removerecords',me,removed,indices);if(autoSync&&sync){me.sync();}},removeAt:function(index){var record=this.getAt(index);if(record){this.remove(record);}},removeAll:function(silent){if(silent!==true){this.fireAction('clear',[this],'doRemoveAll');}else{this.doRemoveAll.call(this,true);}},doRemoveAll:function(silent){var me=this;me.data.each(function(record){record.unjoin(me);});if(me.getSyncRemovedRecords()){me.removed=me.removed.concat(me.data.items);}
me.data.clear();if(silent!==true){me.fireEvent('refresh',me,me.data);}
if(me.getAutoSync()){this.sync();}},each:function(fn,scope){this.data.each(fn,scope);},getCount:function(){return this.data.length||0;},getAt:function(index){return this.data.getAt(index);},getRange:function(start,end){return this.data.getRange(start,end);},getById:function(id){return this.data.findBy(function(record){return record.getId()==id;});},indexOf:function(record){return this.data.indexOf(record);},indexOfId:function(id){return this.data.indexOfKey(id);},afterEdit:function(record,modifiedFieldNames,modified){var me=this,data=me.data,currentId=modified[record.getIdProperty()]||record.getId(),currentIndex=data.keys.indexOf(currentId),newIndex;if(currentIndex===-1&&data.map[currentId]===undefined){return;}
if(me.getAutoSync()){me.sync();}
if(currentId!==record.getId()){data.replace(currentId,record);}else{data.replace(record);}
newIndex=data.indexOf(record);if(currentIndex===-1&&newIndex!==-1){me.fireEvent('addrecords',me,[record]);}
else if(currentIndex!==-1&&newIndex===-1){me.fireEvent('removerecords',me,[record],[currentIndex]);}
else if(newIndex!==-1){me.fireEvent('updaterecord',me,record,newIndex,currentIndex,modifiedFieldNames,modified);}},afterReject:function(record){var index=this.data.indexOf(record);this.fireEvent('updaterecord',this,record,index,index,[],{});},afterCommit:function(record,modifiedFieldNames,modified){var me=this,data=me.data,currentId=modified[record.getIdProperty()]||record.getId(),currentIndex=data.keys.indexOf(currentId),newIndex;if(currentIndex===-1&&data.map[currentId]===undefined){return;}
if(currentId!==record.getId()){data.replace(currentId,record);}else{data.replace(record);}
newIndex=data.indexOf(record);if(currentIndex===-1&&newIndex!==-1){me.fireEvent('addrecords',me,[record]);}
else if(currentIndex!==-1&&newIndex===-1){me.fireEvent('removerecords',me,[record],[currentIndex]);}
else if(newIndex!==-1){me.fireEvent('updaterecord',me,record,newIndex,currentIndex,modifiedFieldNames,modified);}},afterErase:function(record){var me=this,data=me.data,index=data.indexOf(record);if(index!==-1){data.remove(record);me.fireEvent('removerecords',me,[record],[index]);}},updateRemoteFilter:function(remoteFilter){this.data.setAutoFilter(!remoteFilter);},updateRemoteSort:function(remoteSort){this.data.setAutoSort(!remoteSort);},sort:function(sorters,defaultDirection,where){var data=this.data,grouper=this.getGrouper(),autoSort=data.getAutoSort();if(sorters){data.setAutoSort(false);if(typeof where==='string'){if(where=='prepend'){data.insertSorters(grouper?1:0,sorters,defaultDirection);}else{data.addSorters(sorters,defaultDirection);}}else{data.setSorters(null);if(grouper){data.addSorters(grouper);}
data.addSorters(sorters,defaultDirection);}
this.updateSortTypes();data.setAutoSort(autoSort);}
if(!this.getRemoteSort()){if(!sorters){this.data.sort();}
this.fireEvent('sort',this,this.data,this.data.getSorters());if(data.length){this.fireEvent('refresh',this,this.data);}}},filter:function(property,value,anyMatch,caseSensitive){var data=this.data,ln=data.length;if(this.getRemoteFilter()){if(property){if(Ext.isString(property)){data.addFilters({property:property,value:value,anyMatch:anyMatch,caseSensitive:caseSensitive});}
else if(Ext.isArray(property)||property.isFilter){data.addFilters(property);}}}else{data.filter(property,value,anyMatch,caseSensitive);this.fireEvent('filter',this,data,data.getFilters());if(data.length!==ln){this.fireEvent('refresh',this,data);}}},filterBy:function(fn,scope){var me=this,data=me.data,ln=data.length;data.filter({filterFn:fn,scope:scope});this.fireEvent('filter',this,data,data.getFilters());if(data.length!==ln){this.fireEvent('refresh',this,data);}},queryBy:function(fn,scope){return this.data.filterBy(fn,scope||this);},clearFilter:function(suppressEvent){var ln=this.data.length;if(suppressEvent){this.suspendEvents();}
this.data.setFilters(null);if(suppressEvent){this.resumeEvents();}else if(ln!==this.data.length){this.fireEvent('refresh',this,this.data);}},isFiltered:function(){return this.data.filtered;},getSorters:function(){var sorters=this.data.getSorters();return(sorters)?sorters.items:[];},getFilters:function(){var filters=this.data.getFilters();return(filters)?filters.items:[];},getGroups:function(requestGroupString){var records=this.data.items,length=records.length,grouper=this.getGrouper(),groups=[],pointers={},record,groupStr,group,i;if(!grouper){Ext.Logger.error('Trying to get groups for a store that has no grouper');}
for(i=0;i<length;i++){record=records[i];groupStr=grouper.getGroupString(record);group=pointers[groupStr];if(group===undefined){group={name:groupStr,children:[]};groups.push(group);pointers[groupStr]=group;}
group.children.push(record);}
return requestGroupString?pointers[requestGroupString]:groups;},getGroupString:function(record){var grouper=this.getGrouper();if(grouper){return grouper.getGroupString(record);}
return null;},find:function(fieldName,value,startIndex,anyMatch,caseSensitive,exactMatch){var filter=Ext.create('Ext.util.Filter',{property:fieldName,value:value,anyMatch:anyMatch,caseSensitive:caseSensitive,exactMatch:exactMatch,root:'data'});return this.data.findIndexBy(filter.getFilterFn(),null,startIndex);},findRecord:function(){var me=this,index=me.find.apply(me,arguments);return index!==-1?me.getAt(index):null;},findExact:function(fieldName,value,startIndex){return this.data.findIndexBy(function(record){return record.get(fieldName)===value;},this,startIndex);},findBy:function(fn,scope,startIndex){return this.data.findIndexBy(fn,scope,startIndex);},load:function(options,scope){var me=this,operation,currentPage=me.currentPage,pageSize=me.getPageSize();options=options||{};if(Ext.isFunction(options)){options={callback:options,scope:scope||this};}
if(me.getRemoteSort()){options.sorters=options.sorters||this.getSorters();}
if(me.getRemoteFilter()){options.filters=options.filters||this.getFilters();}
if(me.getRemoteGroup()){options.grouper=options.grouper||this.getGrouper();}
Ext.applyIf(options,{page:currentPage,start:(currentPage-1)*pageSize,limit:pageSize,addRecords:false,action:'read',model:this.getModel()});operation=Ext.create('Ext.data.Operation',options);if(me.fireEvent('beforeload',me,operation)!==false){me.loading=true;me.getProxy().read(operation,me.onProxyLoad,me);}
return me;},isLoading:function(){return Boolean(this.loading);},isLoaded:function(){return Boolean(this.loaded);},sync:function(){var me=this,operations={},toCreate=me.getNewRecords(),toUpdate=me.getUpdatedRecords(),toDestroy=me.getRemovedRecords(),needsSync=false;if(toCreate.length>0){operations.create=toCreate;needsSync=true;}
if(toUpdate.length>0){operations.update=toUpdate;needsSync=true;}
if(toDestroy.length>0){operations.destroy=toDestroy;needsSync=true;}
if(needsSync&&me.fireEvent('beforesync',this,operations)!==false){me.getProxy().batch({operations:operations,listeners:me.getBatchListeners()});}
return{added:toCreate,updated:toUpdate,removed:toDestroy};},first:function(){return this.data.first();},last:function(){return this.data.last();},sum:function(field){var total=0,i=0,records=this.data.items,len=records.length;for(;i<len;++i){total+=records[i].get(field);}
return total;},min:function(field){var i=1,records=this.data.items,len=records.length,value,min;if(len>0){min=records[0].get(field);}
for(;i<len;++i){value=records[i].get(field);if(value<min){min=value;}}
return min;},max:function(field){var i=1,records=this.data.items,len=records.length,value,max;if(len>0){max=records[0].get(field);}
for(;i<len;++i){value=records[i].get(field);if(value>max){max=value;}}
return max;},average:function(field){var i=0,records=this.data.items,len=records.length,sum=0;if(records.length>0){for(;i<len;++i){sum+=records[i].get(field);}
return sum/len;}
return 0;},getBatchListeners:function(){return{scope:this,exception:this.onBatchException,complete:this.onBatchComplete};},onBatchComplete:function(batch){var me=this,operations=batch.operations,length=operations.length,i;for(i=0;i<length;i++){me.onProxyWrite(operations[i]);}},onBatchException:function(batch,operation){},onProxyLoad:function(operation){var me=this,records=operation.getRecords(),resultSet=operation.getResultSet(),successful=operation.wasSuccessful();if(resultSet){me.setTotalCount(resultSet.getTotal());}
if(successful){this.fireAction('datarefresh',[this,this.data,operation],'doDataRefresh');}
me.loaded=true;me.loading=false;me.fireEvent('load',this,records,successful,operation);Ext.callback(operation.getCallback(),operation.getScope()||me,[records,operation,successful]);},doDataRefresh:function(store,data,operation){var records=operation.getRecords(),me=this;if(operation.getAddRecords()!==true){data.each(function(record){record.unjoin(me);});data.clear();me.fireEvent('clear',this);}
if(records&&records.length){me.suspendEvents();me.add(records);me.resumeEvents();}
this.fireEvent('refresh',this,this.data);},onProxyWrite:function(operation){var me=this,success=operation.wasSuccessful(),records=operation.getRecords();switch(operation.getAction()){case'create':me.onCreateRecords(records,operation,success);break;case'update':me.onUpdateRecords(records,operation,success);break;case'destroy':me.onDestroyRecords(records,operation,success);break;}
if(success){me.fireEvent('write',me,operation);}
Ext.callback(operation.getCallback(),operation.getScope()||me,[records,operation,success]);},onCreateRecords:function(records,operation,success){},onUpdateRecords:function(records,operation,success){},onDestroyRecords:function(records,operation,success){this.removed=[];},onMetaChange:function(data){var model=this.getProxy().getModel();if(!this.getModel()&&model){this.setModel(model);}
this.fireEvent('metachange',this,data);},getNewRecords:function(){return this.data.filterBy(function(item){return item.phantom===true&&item.isValid();}).items;},getUpdatedRecords:function(){return this.data.filterBy(function(item){return item.dirty===true&&item.phantom!==true&&item.isValid();}).items;},getRemovedRecords:function(){return this.removed;},loadPage:function(page,options,scope){if(typeof options==='function'){options={callback:options,scope:scope||this};}
var me=this,pageSize=me.getPageSize(),clearOnPageLoad=me.getClearOnPageLoad();options=Ext.apply({},options);me.currentPage=page;me.load(Ext.applyIf(options,{page:page,start:(page-1)*pageSize,limit:pageSize,addRecords:!clearOnPageLoad}));},nextPage:function(options){this.loadPage(this.currentPage+1,options);},previousPage:function(options){this.loadPage(this.currentPage-1,options);},onClassExtended:function(cls,data){var prototype=this.prototype,defaultConfig=prototype.config,config=data.config||{},key;for(key in defaultConfig){if(key!="control"&&key in data){config[key]=data[key];delete data[key];Ext.Logger.deprecate(key+' is deprecated as a property directly on the '+this.$className+' prototype. Please put it inside the config object.');}}
data.config=config;}},function(){this.override({loadData:function(data,append){Ext.Logger.deprecate("loadData is deprecated, please use either add or setData");if(append){this.add(data);}else{this.setData(data);}},doAddListener:function(name,fn,scope,options,order){switch(name){case'update':Ext.Logger.warn('The update event on Store has been removed. Please use the updaterecord event from now on.');return this;case'add':Ext.Logger.warn('The add event on Store has been removed. Please use the addrecords event from now on.');return this;case'remove':Ext.Logger.warn('The remove event on Store has been removed. Please use the removerecords event from now on.');return this;case'datachanged':Ext.Logger.warn('The datachanged event on Store has been removed. Please use the refresh event from now on.');return this;break;}
return this.callParent(arguments);}});Ext.deprecateMethod(this,'loadRecords','add',"Ext.data.Store#loadRecords has been deprecated. Please use the add method.");});Ext.define('Ext.data.proxy.WebStorage',{extend:'Ext.data.proxy.Client',alternateClassName:'Ext.data.WebStorageProxy',requires:'Ext.Date',config:{id:undefined,reader:null,writer:null},constructor:function(config){this.callParent(arguments);this.cache={};if(this.getStorageObject()===undefined){Ext.Logger.error("Local Storage is not supported in this browser, please use another type of data proxy");}},updateModel:function(model){if(!this.getId()){this.setId(model.modelName);}},create:function(operation,callback,scope){var records=operation.getRecords(),length=records.length,ids=this.getIds(),id,record,i;operation.setStarted();for(i=0;i<length;i++){record=records[i];if(record.phantom){record.phantom=false;id=this.getNextId();}else{id=record.getId();}
this.setRecord(record,id);ids.push(id);}
this.setIds(ids);operation.setCompleted();operation.setSuccessful();if(typeof callback=='function'){callback.call(scope||this,operation);}},read:function(operation,callback,scope){var records=[],ids=this.getIds(),model=this.getModel(),idProperty=model.getIdProperty(),params=operation.getParams()||{},length=ids.length,i,record;if(params[idProperty]!==undefined){record=this.getRecord(params[idProperty]);if(record){records.push(record);operation.setSuccessful();}}else{for(i=0;i<length;i++){records.push(this.getRecord(ids[i]));}
operation.setSuccessful();}
operation.setCompleted();operation.setResultSet(Ext.create('Ext.data.ResultSet',{records:records,total:records.length,loaded:true}));operation.setRecords(records);if(typeof callback=='function'){callback.call(scope||this,operation);}},update:function(operation,callback,scope){var records=operation.getRecords(),length=records.length,ids=this.getIds(),record,id,i;operation.setStarted();for(i=0;i<length;i++){record=records[i];this.setRecord(record);id=record.getId();if(id!==undefined&&Ext.Array.indexOf(ids,id)==-1){ids.push(id);}}
this.setIds(ids);operation.setCompleted();operation.setSuccessful();if(typeof callback=='function'){callback.call(scope||this,operation);}},destroy:function(operation,callback,scope){var records=operation.getRecords(),length=records.length,ids=this.getIds(),newIds=[].concat(ids),i;for(i=0;i<length;i++){Ext.Array.remove(newIds,records[i].getId());this.removeRecord(records[i],false);}
this.setIds(newIds);operation.setCompleted();operation.setSuccessful();if(typeof callback=='function'){callback.call(scope||this,operation);}},getRecord:function(id){if(this.cache[id]===undefined){var recordKey=this.getRecordKey(id),item=this.getStorageObject().getItem(recordKey),data={},Model=this.getModel(),fields=Model.getFields().items,length=fields.length,i,field,name,record,rawData,dateFormat;if(!item){return;}
rawData=Ext.decode(item);for(i=0;i<length;i++){field=fields[i];name=field.getName();if(typeof field.getDecode()=='function'){data[name]=field.getDecode()(rawData[name]);}else{if(field.getType().type=='date'){dateFormat=field.getDateFormat();if(dateFormat){data[name]=Ext.Date.parse(rawData[name],dateFormat);}else{data[name]=new Date(rawData[name]);}}else{data[name]=rawData[name];}}}
record=new Model(data,id);this.cache[id]=record;}
return this.cache[id];},setRecord:function(record,id){if(id){record.setId(id);}else{id=record.getId();}
var me=this,rawData=record.getData(),data={},Model=me.getModel(),fields=Model.getFields().items,length=fields.length,i=0,field,name,obj,key,dateFormat;for(;i<length;i++){field=fields[i];name=field.getName();if(typeof field.getEncode()=='function'){data[name]=field.getEncode()(rawData[name],record);}else{if(field.getType().type=='date'&&Ext.isDate(rawData[name])){dateFormat=field.getDateFormat();if(dateFormat){data[name]=Ext.Date.format(rawData[name],dateFormat);}else{data[name]=rawData[name].getTime();}}else{data[name]=rawData[name];}}}
obj=me.getStorageObject();key=me.getRecordKey(id);me.cache[id]=record;obj.removeItem(key);obj.setItem(key,Ext.encode(data));},removeRecord:function(id,updateIds){var me=this,ids;if(id.isModel){id=id.getId();}
if(updateIds!==false){ids=me.getIds();Ext.Array.remove(ids,id);me.setIds(ids);}
me.getStorageObject().removeItem(me.getRecordKey(id));},getRecordKey:function(id){if(id.isModel){id=id.getId();}
return Ext.String.format("{0}-{1}",this.getId(),id);},getRecordCounterKey:function(){return Ext.String.format("{0}-counter",this.getId());},getIds:function(){var ids=(this.getStorageObject().getItem(this.getId())||"").split(","),length=ids.length,i;if(length==1&&ids[0]===""){ids=[];}else{for(i=0;i<length;i++){ids[i]=parseInt(ids[i],10);}}
return ids;},setIds:function(ids){var obj=this.getStorageObject(),str=ids.join(","),id=this.getId(),key=this.getRecordCounterKey();obj.removeItem(id);if(Ext.isEmpty(str)){obj.removeItem(key);}else{obj.setItem(id,str);}},getNextId:function(){var obj=this.getStorageObject(),key=this.getRecordCounterKey(),last=obj.getItem(key),ids,id;if(last===null){ids=this.getIds();last=ids[ids.length-1]||0;}
id=parseInt(last,10)+1;obj.setItem(key,id);return id;},initialize:function(){this.callParent(arguments);var storageObject=this.getStorageObject();storageObject.setItem(this.getId(),storageObject.getItem(this.getId())||"");},clear:function(){var obj=this.getStorageObject(),ids=this.getIds(),len=ids.length,i;for(i=0;i<len;i++){this.removeRecord(ids[i]);}
obj.removeItem(this.getRecordCounterKey());obj.removeItem(this.getId());},getStorageObject:function(){Ext.Logger.error("The getStorageObject function has not been defined in your Ext.data.proxy.WebStorage subclass");}});Ext.define('Ext.data.proxy.LocalStorage',{extend:'Ext.data.proxy.WebStorage',alias:'proxy.localstorage',alternateClassName:'Ext.data.LocalStorageProxy',getStorageObject:function(){return window.localStorage;}});Ext.define('zvsMobile.model.Settings',{extend:'Ext.data.Model',config:{fields:[{name:"id"},{name:"SettingName",type:"string"},{name:"Value",type:"string"}],proxy:{type:'localstorage',id:'zvs-settings'}}});Ext.define('zvsMobile.store.Settings',{extend:'Ext.data.Store',requires:['zvsMobile.model.Settings','Ext.data.Batch'],config:{model:'zvsMobile.model.Settings',autoLoad:true}});Ext.create('zvsMobile.store.Settings',{id:'appSettingsStore',model:'zvsMobile.model.Settings'});Ext.define('zvsMobile.model.Device',{extend:'Ext.data.Model',config:{fields:['id','name','on_off','level','level_txt','type']}});Ext.define('zvsMobile.store.Devices',{extend:'Ext.data.Store',requires:['zvsMobile.model.Device'],config:{model:'zvsMobile.model.Device'}});var DeviceStore=Ext.create('zvsMobile.store.Devices',{id:'DeviceStore',requires:['zvsMobile.store.Devices']});Ext.define('zvsMobile.model.Group',{extend:'Ext.data.Model',config:{fields:['id','name','count']}});Ext.define('zvsMobile.store.Groups',{extend:'Ext.data.Store',requires:['zvsMobile.model.Group'],config:{model:'zvsMobile.model.Group'}});var GroupStore=Ext.create('zvsMobile.store.Groups',{id:'GroupStore',requires:['zvsMobile.store.Groups']});Ext.define('zvsMobile.model.Scene',{extend:'Ext.data.Model',config:{fields:[{name:'id',type:'int'},{name:'name',type:'string'},{name:'is_running',type:'bool'},{name:'cmd_count',type:'string'}]}});Ext.define('zvsMobile.store.Scenes',{extend:'Ext.data.Store',requires:['zvsMobile.model.Scene'],config:{model:'zvsMobile.model.Scene'}});SceneStore=Ext.create('zvsMobile.store.Scenes',{id:'SceneStore',requires:['zvsMobile.store.Scenes']});Ext.define('zvsMobile.view.SettingsLogOut',{extend:'Ext.Panel',xtype:'LogOut',constructor:function(config){var self=this;Ext.apply(config||{},{UpdateLogoutHTML:function()
{console.log(self.items.items[0]);self.items.items[0].updateHtml('<div class="logout_info"><p> Logged in to: '+zvsMobile.app.BaseURL()+'</p></div>')},items:[{xtype:'panel',html:''},{xtype:'fieldset',style:'padding:10px;',items:[{xtype:'button',text:'Logout',width:'90%',style:'margin:10px auto;',handler:function(b){Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/logout',method:'POST',params:{u:Math.random()},success:function(response,opts){var result=JSON.parse(response.responseText);if(result.success){self.fireEvent('loggedOut');}
else{Ext.Msg.alert('Logout failed.','Please try again.');}},failure:function(result,request){Ext.Msg.alert('Logout failed.','Please try again.');}});}}]}]});this.callParent([config]);},config:{xtype:'panel',layout:'vbox',scrollable:'vertical'}});Ext.define('zvsMobile.view.DeviceDetailsThermo',{extend:'Ext.Panel',xtype:'DeviceDetailsThermo',constructor:function(config){var self=this;self.RepollTimer;self.deviceID=0;Ext.apply(config||{},{xtype:'panel',layout:'vbox',scrollable:'vertical',items:[{xtype:'panel',id:'ThermoTPL',tpl:new Ext.XTemplate('<div class="device_info">','<div id="level_temp_img" class="imageholder {type}"></div>','<div id="level_temp_details" class="level">{level_txt}</div>','<h1>{name}</h1>','<h2>{type_txt}<h2>','<div class="overview">','<strong>Currently: </strong>{level}&deg; F<br />','<strong>Operating State: </strong>{op_state}<br />','<strong>Fan State: </strong>{fan_state}<br />','<strong>Energy Mode: </strong>','<tpl if="esm == 0">','Energy Savings Mode','<tpl else>','Comfort Mode','</tpl>','<br /><br />','<strong>Mode: </strong>{mode}<br />','<strong>Fan Mode: </strong>{fan_mode}<br />','<strong>Heat Point: </strong>{heat_p}&deg; F<br />','<strong>Cool Point: </strong>{cool_p}&deg; F<br /><br />','<tpl if="groups">','<strong>Groups: </strong>{groups}<br />','</tpl>','<strong>Updated: </strong>{last_heard_from}','</div>','</div>')},{xtype:'button',text:'Energy Mode',ui:'confirm',margin:5,handler:function(){console.log('AJAX: SendCmd ESM');Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/device/'+self.deviceID+'/command/',method:'POST',params:{u:Math.random(),name:'SETENERGYMODE',arg:0,type:'device_type'},success:function(response,opts){var result=JSON.parse(response.responseText);if(result.success){self.delayedReload();}
else{Ext.Msg.alert('Thermostat Command','Communication Error!');}}});}},{xtype:'button',text:'Comfort Mode',ui:'confirm',margin:'5 5 30 5',handler:function(){console.log('AJAX: SendCmd Confort');Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/device/'+self.deviceID+'/command/',method:'POST',params:{u:Math.random(),name:'SETCONFORTMODE',arg:0,type:'device_type'},success:function(response,opts){var result=JSON.parse(response.responseText);if(result.success){self.delayedReload();}
else{Ext.Msg.alert('Thermostat Command','Communication Error!');}}});}},{xtype:'button',text:'Change Mode',ui:'action',margin:5,flex:1,handler:function(){if(!SetMode){var SetMode=Ext.create('Ext.ActionSheet',{items:[{xtype:'selectfield',label:'Mode',margin:'15 5',options:[{text:'Off',value:'Off'},{text:'Auto',value:'Auto'},{text:'Heat',value:'Heat'},{text:'Cool',value:'Cool'}]},{xtype:'toolbar',docked:'top',items:[{xtype:'button',text:'Cancel',scope:this,handler:function(){SetMode.hide();}},{xtype:'spacer'},{xtype:'button',text:'Set Mode',scope:this,handler:function(){var mode=SetMode.items.items[0].getValue()
console.log('AJAX DYNAMIC_CMD_MODE '+mode);Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/device/'+self.deviceID+'/command/',method:'POST',params:{u:Math.random(),name:'DYNAMIC_CMD_MODE',arg:mode,type:'device'},success:function(response,opts){var result=JSON.parse(response.responseText);if(result.success){self.delayedReload();}
else{Ext.Msg.alert('Thermostat Command','Communication Error!');}}});SetMode.hide();}}]}]});}
Ext.Viewport.add(SetMode);SetMode.show();var ThermoTPL=self.items.items[1];var data=ThermoTPL.getData();SetMode.items.items[0].setValue(data.mode)}},{xtype:'button',text:'Change Fan Mode',ui:'action',margin:5,flex:1,handler:function(){if(!SetFanMode){var SetFanMode=Ext.create('Ext.ActionSheet',{items:[{xtype:'selectfield',label:'Fan Mode',margin:'15 5',options:[{text:'On Low',value:'On Low'},{text:'Auto Low',value:'Auto Low'}]},{xtype:'toolbar',docked:'top',items:[{xtype:'button',text:'Cancel',scope:this,handler:function(){SetFanMode.hide();}},{xtype:'spacer'},{xtype:'button',text:'Set Fan Mode',scope:this,handler:function(){var mode=SetFanMode.items.items[0].getValue()
console.log('DYNAMIC_CMD_FAN MODE'+mode);Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/device/'+self.deviceID+'/command/',method:'POST',params:{u:Math.random(),name:'DYNAMIC_CMD_FAN MODE',arg:mode,type:'device'},success:function(response,opts){var result=JSON.parse(response.responseText);if(result.success){self.delayedReload();}
else{Ext.Msg.alert('Thermostat Command','Communication Error!');}}});SetFanMode.hide();}}]}]});}
Ext.Viewport.add(SetFanMode);SetFanMode.show();var ThermoTPL=Ext.getCmp('ThermoTPL');var data=ThermoTPL.getData();SetFanMode.items.items[0].setValue(data.fan_mode)}},{xtype:'button',text:'Change Heat Point',ui:'action',margin:5,flex:1,handler:function(){var picker=Ext.create('Ext.Picker',{slots:[{name:'temperature',title:'Temperature',data:tempSetPoints}],doneButton:{xtype:'button',text:'Set Heat Point',handler:function(){var selected_temp=picker._slots[0].picker._values.temperature;console.log('AJAX DYNAMIC_CMD_HEATING 1'+selected_temp);Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/device/'+self.deviceID+'/command/',method:'POST',params:{u:Math.random(),name:'DYNAMIC_CMD_HEATING 1',arg:selected_temp,type:'device'},success:function(response,opts){var result=JSON.parse(response.responseText);if(result.success){self.delayedReload();}
else{Ext.Msg.alert('Thermostat Command','Communication Error!');}}});}}});Ext.Viewport.add(picker);picker.show();var ThermoTPL=Ext.getCmp('ThermoTPL');var data=ThermoTPL.getData();picker.setValue({temperature:data.heat_p},true)}},{xtype:'button',text:'Change Cool Point',ui:'action',margin:5,flex:1,handler:function(){var picker=Ext.create('Ext.Picker',{slots:[{name:'temperature',title:'Temperature',data:tempSetPoints}],doneButton:{xtype:'button',text:'Set Cool Point',handler:function(){var selected_temp=picker._slots[0].picker._values.temperature;console.log('AJAX DYNAMIC_CMD_COOLING 1 :'+selected_temp);Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/device/'+self.deviceID+'/command/',method:'POST',params:{u:Math.random(),name:'DYNAMIC_CMD_COOLING 1',arg:selected_temp,type:'device'},success:function(response,opts){var result=JSON.parse(response.responseText);if(result.success){self.delayedReload();}
else{Ext.Msg.alert('Thermostat Command','Communication Error!');}}});}}});Ext.Viewport.add(picker);picker.show();var ThermoTPL=Ext.getCmp('ThermoTPL');var data=ThermoTPL.getData();picker.setValue({temperature:data.cool_p},true)}},{xtype:'button',text:'Repoll',ui:'confirm',margin:'30 5 5 5',flex:1,handler:function(){console.log('AJAX: SendCmd REPOLL_ME');Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/commands/',method:'POST',params:{u:Math.random(),name:'REPOLL_ME',arg:self.deviceID},success:function(response,opts){var result=JSON.parse(response.responseText);if(result.success){self.delayedReload();}
else{console.log('ERROR');}}});}}],listeners:{scope:this,deactivate:function(){if(self.RepollTimer){clearInterval(self.RepollTimer);}}}});this.callOverridden([config]);},delayedReload:function(){var self=this;var ThermoTPL=Ext.getCmp('ThermoTPL');if(self.RepollTimer){clearInterval(self.RepollTimer);}
self.RepollTimer=setTimeout(function(){self.loadDevice(self.deviceID);},1500);},loadDevice:function(deviceId){var self=this;var ThermoTPL=Ext.getCmp('ThermoTPL');self.deviceID=deviceId;console.log('AJAX: GetDeviceDetails');Ext.data.JsonP.request({url:zvsMobile.app.BaseURL()+'/device/'+deviceId,callbackKey:'callback',params:{u:Math.random()},success:function(result){ThermoTPL.setData(result.details);self.UpdateLevel(result.details.level);}});},UpdateLevel:function(value){var self=this;var ThermoTPL=Ext.getCmp('ThermoTPL');var data=Ext.clone(ThermoTPL._data);data.level=value;data.level_txt=value+'F';ThermoTPL.setData(data);data=DeviceStore.data.items;for(i=0,len=data.length;i<len;i++){if(data[i].data.id===ThermoTPL._data.id){data[i].data.level=value;data[i].data.level_txt=value+'F';}}
DeviceStore.add(data);Ext.getCmp('DeviceList').refresh();}});var tempSetPoints=[{text:'40&deg;',value:40},{text:'41&deg;',value:41},{text:'42&deg;',value:42},{text:'43&deg;',value:43},{text:'44&deg;',value:44},{text:'45&deg;',value:45},{text:'46&deg;',value:46},{text:'47&deg;',value:47},{text:'48&deg;',value:48},{text:'49&deg;',value:49},{text:'50&deg;',value:50},{text:'51&deg;',value:51},{text:'52&deg;',value:52},{text:'53&deg;',value:53},{text:'54&deg;',value:54},{text:'55&deg;',value:55},{text:'56&deg;',value:56},{text:'57&deg;',value:57},{text:'58&deg;',value:58},{text:'59&deg;',value:59},{text:'60&deg;',value:60},{text:'61&deg;',value:61},{text:'62&deg;',value:62},{text:'63&deg;',value:63},{text:'64&deg;',value:64},{text:'65&deg;',value:65},{text:'66&deg;',value:66},{text:'67&deg;',value:67},{text:'68&deg;',value:68},{text:'69&deg;',value:69},{text:'70&deg;',value:70},{text:'71&deg;',value:71},{text:'72&deg;',value:72},{text:'73&deg;',value:73},{text:'74&deg;',value:74},{text:'75&deg;',value:75},{text:'76&deg;',value:76},{text:'77&deg;',value:77},{text:'78&deg;',value:78},{text:'79&deg;',value:79},{text:'80&deg;',value:80},{text:'81&deg;',value:81},{text:'82&deg;',value:82},{text:'83&deg;',value:83},{text:'84&deg;',value:84},{text:'85&deg;',value:85},{text:'86&deg;',value:86},{text:'87&deg;',value:87},{text:'88&deg;',value:88},{text:'89&deg;',value:89},{text:'90&deg;',value:90},{text:'91&deg;',value:91},{text:'92&deg;',value:92},{text:'93&deg;',value:93},{text:'94&deg;',value:94},{text:'95&deg;',value:95},{text:'96&deg;',value:96},{text:'97&deg;',value:97},{text:'98&deg;',value:98},{text:'99&deg;',value:99}];Ext.define('zvsMobile.view.SceneDetails',{extend:'Ext.Panel',xtype:'SceneDetails',constructor:function(config){var RepollTimer;var sceneId=0;var self=this;Ext.apply(config||{},{items:[{xtype:'panel',id:'SceneStatusTPL',tpl:new Ext.XTemplate('<div class="scene_info">','<div class="head">','<div class="image s_img_{scene.is_running}"></div>','<tpl for="scene">','<h1>{name}</h1>','<div class="scene_overview"><strong>Running:</strong>','<tpl if="is_running">Yes<tpl else>No</tpl></div>','</div>','</tpl>','</div>')},{xtype:'button',id:'activeSceneButton',text:'Activate',ui:'confirm',margin:'25 5',handler:function(){console.log('AJAX: ActivateScene');Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/scene/'+self.sceneId,method:'POST',params:{is_running:true},success:function(response,opts){var result=JSON.parse(response.responseText);if(result.success){var SceneStatusTPL=Ext.getCmp('SceneStatusTPL');SceneStatusTPL._data.scene.is_running=true;SceneStatusTPL.setData(SceneStatusTPL._data);data=SceneStore.data.items;for(i=0,len=data.length;i<len;i++){if(data[i].data.id===SceneStatusTPL._data.scene.id){data[i].data.is_running=true;}}
SceneStore.add(data);Ext.getCmp('SceneList').refresh();self.delayedReload();Ext.getCmp('SceneActiveResult').setHtml(result.desc);}
else{Ext.Msg.alert('Scene Activation','Communication Error!');}}});}},{xtype:'panel',id:'SceneActiveResult',cls:'result',html:''},{xtype:'panel',id:'ScenesDetailsTPL',tpl:new Ext.XTemplate('<tpl for="scene">','<tpl if="cmd_count &gt; 0">','<div class="scene_overview">','<table class="info">','<thead>','<tr>','<th></th>','<th scope="col" abbr="Device">Device / Cmd</th>','<th scope="col" abbr="Action">Action</th>','</tr>','</thead>','<tbody>','<tpl for="cmds">','<tr>','<th scope="row">{order}</th>','<td>{device}</td>','<td>{action}</td>','</tr>','</tpl>','</tbody>','</table>','</div>','</tpl>','</tpl>')}]});this.callOverridden([config]);},config:{layout:'vbox',scrollable:'vertical'},delayedReload:function(){var self=this;if(self.RepollTimer){clearInterval(self.RepollTimer);}
self.RepollTimer=setTimeout(function(){self.loadScene(self.sceneId);SceneStore.load();},5000);},loadScene:function(sceneId){var self=this;self.sceneId=sceneId;console.log('AJAX: GetSceneDetails');Ext.data.JsonP.request({url:zvsMobile.app.BaseURL()+'/scene/'+sceneId,callbackKey:'callback',params:{u:Math.random()},success:function(result){var ScenesDetailsTPL=Ext.getCmp('ScenesDetailsTPL');var SceneStatusTPL=Ext.getCmp('SceneStatusTPL');SceneStatusTPL.setData(result);ScenesDetailsTPL.setData(result);data=SceneStore.data.items;for(i=0,len=data.length;i<len;i++){if(data[i].data.id===SceneStatusTPL._data.scene.id){data[i].data.is_running=result.scene.is_running;}}
SceneStore.add(data);Ext.getCmp('SceneList').refresh();Ext.getCmp('SceneActiveResult').setHtml('');}});}});Ext.define('zvsMobile.view.GroupDetails',{extend:'Ext.Panel',xtype:'GroupDetails',constructor:function(config){var self=this;var groupId=0;var RepollTimer;Ext.apply(config||{},{items:[{xtype:'panel',id:'groupStatusTPL',tpl:new Ext.XTemplate('<div class="group_info">','<div class="head">','<div class="image"></div>','<tpl for="group">','<h1>{name}</h1>','</tpl>','</div>','</div>')},{xtype:'button',text:'Turn On',ui:'confirm',margin:'25 5 5 5',handler:function(){console.log('AJAX: ActivateGroup'+self.groupId);Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/commands/',method:'POST',params:{u:Math.random(),name:'GROUP_ON',arg:self.groupId},success:function(response,opts){var result=JSON.parse(response.responseText);if(result.success){Ext.getCmp('GroupActiveResult').setHtml('All device in group Turned On.');}
else{Ext.Msg.alert('Group','Communication Error!');}}});}},{xtype:'button',text:'Turn Off',ui:'confirm',margin:'5 5 25 5',handler:function(){console.log('AJAX: DeactivateGroup'+self.groupId);Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/commands/',method:'POST',params:{u:Math.random(),name:'GROUP_OFF',arg:self.groupId},success:function(response,opts){var result=JSON.parse(response.responseText);if(result.success){Ext.getCmp('GroupActiveResult').setHtml('All device in group Turned Off.');}
else{Ext.Msg.alert('Group','Communication Error!');}}});}},{xtype:'panel',id:'GroupActiveResult',cls:'result',html:''},{xtype:'panel',id:'groupDetailsTPL',tpl:new Ext.XTemplate('<tpl for="group">','<div class="group_overview">','<table class="info">','<thead>','<tr>','<th scope="col" abbr="Device">Device</th>','<th scope="col" abbr="Action">Type</th>','</tr>','</thead>','<tbody>','<tpl for="devices">','<tr>','<td>{name}</td>','<td>{type}</td>','</tr>','</tpl>','</tbody>','</table>','</div>','</tpl>')}]});this.callOverridden([config]);},config:{layout:'vbox',scrollable:'vertical'},loadGroup:function(groupID){var self=this;self.groupId=groupID;Ext.getCmp('GroupActiveResult').setHtml('');console.log('AJAX: GetgroupDetails');Ext.data.JsonP.request({url:zvsMobile.app.BaseURL()+'/group/'+self.groupId,callbackKey:'callback',params:{u:Math.random()},success:function(result){Ext.getCmp('groupDetailsTPL').setData(result);Ext.getCmp('groupStatusTPL').setData(result);}});}});Ext.define('Ext.field.Password',{extend:'Ext.field.Text',xtype:'passwordfield',alternateClassName:'Ext.form.Password',config:{autoCapitalize:false,component:{type:'password'}}});Ext.define('Ext.form.FieldSet',{extend:'Ext.Container',alias:'widget.fieldset',requires:['Ext.Title'],config:{baseCls:Ext.baseCSSPrefix+'form-fieldset',title:null,instructions:null},applyTitle:function(title){if(typeof title=='string'){title={title:title};}
Ext.applyIf(title,{docked:'top',baseCls:this.getBaseCls()+'-title'});return Ext.factory(title,Ext.Title,this.getTitle());},updateTitle:function(newTitle,oldTitle){if(newTitle){this.add(newTitle);}
if(oldTitle){this.remove(oldTitle);}},applyInstructions:function(instructions){if(typeof instructions=='string'){instructions={title:instructions};}
Ext.applyIf(instructions,{docked:'bottom',baseCls:this.getBaseCls()+'-instructions'});return Ext.factory(instructions,Ext.Title,this.getInstructions());},updateInstructions:function(newInstructions,oldInstructions){if(newInstructions){this.add(newInstructions);}
if(oldInstructions){this.remove(oldInstructions);}}});Ext.define('Ext.plugin.PullRefresh',{extend:'Ext.Component',alias:'plugin.pullrefresh',requires:['Ext.DateExtras'],config:{list:null,pullRefreshText:'Pull down to refresh...',releaseRefreshText:'Release to refresh...',loadingText:'Loading...',snappingAnimationDuration:150,refreshFn:null,pullTpl:['<div class="x-list-pullrefresh">','<div class="x-list-pullrefresh-arrow"></div>','<div class="x-loading-spinner">','<span class="x-loading-top"></span>','<span class="x-loading-right"></span>','<span class="x-loading-bottom"></span>','<span class="x-loading-left"></span>','</div>','<div class="x-list-pullrefresh-wrap">','<h3 class="x-list-pullrefresh-message">{message}</h3>','<div class="x-list-pullrefresh-updated">Last Updated: <span>{lastUpdated:date("m/d/Y h:iA")}</span></div>','</div>','</div>'].join('')},isRefreshing:false,currentViewState:'',initialize:function(){this.callParent();this.on({painted:'onPainted',scope:this});},init:function(list){var me=this,store=list.getStore(),pullTpl=me.getPullTpl(),element=me.element,scroller=list.getScrollable().getScroller();me.setList(list);me.lastUpdated=new Date();list.insert(0,me);if(store){if(store.isAutoLoading()){list.setLoadingText(null);}else{store.on({load:{single:true,fn:function(){list.setLoadingText(null);}}});}}
pullTpl.overwrite(element,{message:me.getPullRefreshText(),lastUpdated:me.lastUpdated},true);me.loadingElement=element.getFirstChild();me.messageEl=element.down('.x-list-pullrefresh-message');me.updatedEl=element.down('.x-list-pullrefresh-updated > span');me.maxScroller=scroller.getMaxPosition();scroller.on({maxpositionchange:me.setMaxScroller,scroll:me.onScrollChange,scope:me});},fetchLatest:function(){var store=this.getList().getStore(),proxy=store.getProxy(),operation;operation=Ext.create('Ext.data.Operation',{page:1,start:0,model:store.getModel(),limit:store.getPageSize(),action:'read',filters:store.getRemoteFilter()?store.getFilters():[]});proxy.read(operation,this.onLatestFetched,this);},onLatestFetched:function(operation){var store=this.getList().getStore(),oldRecords=store.getData(),newRecords=operation.getRecords(),length=newRecords.length,toInsert=[],newRecord,oldRecord,i;for(i=0;i<length;i++){newRecord=newRecords[i];oldRecord=oldRecords.getByKey(newRecord.getId());if(oldRecord){oldRecord.set(newRecord.getData());}else{toInsert.push(newRecord);}
oldRecord=undefined;}
store.insert(0,toInsert);},onPainted:function(){this.pullHeight=this.loadingElement.getHeight();},setMaxScroller:function(scroller,position){this.maxScroller=position;},onScrollChange:function(scroller,x,y){if(y<0){this.onBounceTop(y);}
if(y>this.maxScroller.y){this.onBounceBottom(y);}},applyPullTpl:function(config){return(Ext.isObject(config)&&config.isTemplate)?config:new Ext.XTemplate(config);},onBounceTop:function(y){var me=this,list=me.getList(),scroller=list.getScrollable().getScroller();if(!me.isReleased){if(!me.isRefreshing&&-y>=me.pullHeight+10){me.isRefreshing=true;me.setViewState('release');scroller.getContainer().onBefore({dragend:'onScrollerDragEnd',single:true,scope:me});}
else if(me.isRefreshing&&-y<me.pullHeight+10){me.isRefreshing=false;me.setViewState('pull');}}},onScrollerDragEnd:function(){var me=this;if(me.isRefreshing){var list=me.getList(),scroller=list.getScrollable().getScroller();scroller.minPosition.y=-me.pullHeight;scroller.on({scrollend:'loadStore',single:true,scope:me});me.isReleased=true;}},loadStore:function(){var me=this,list=me.getList(),scroller=list.getScrollable().getScroller();me.setViewState('loading');me.isReleased=false;Ext.defer(function(){scroller.on({scrollend:function(){if(me.getRefreshFn()){me.getRefreshFn().call(me,me);}else{me.fetchLatest();}
me.resetRefreshState();},delay:100,single:true,scope:me});scroller.minPosition.y=0;scroller.scrollTo(null,0,true);},500,me);},onBounceBottom:Ext.emptyFn,setViewState:function(state){var me=this,prefix=Ext.baseCSSPrefix,messageEl=me.messageEl,loadingElement=me.loadingElement;if(state===me.currentViewState){return me;}
me.currentViewState=state;if(messageEl&&loadingElement){switch(state){case'pull':messageEl.setHtml(me.getPullRefreshText());loadingElement.removeCls([prefix+'list-pullrefresh-release',prefix+'list-pullrefresh-loading']);break;case'release':messageEl.setHtml(me.getReleaseRefreshText());loadingElement.addCls(prefix+'list-pullrefresh-release');break;case'loading':messageEl.setHtml(me.getLoadingText());loadingElement.addCls(prefix+'list-pullrefresh-loading');break;}}
return me;},resetRefreshState:function(){var me=this;me.isRefreshing=false;me.lastUpdated=new Date();me.setViewState('pull');me.updatedEl.setHtml(Ext.util.Format.date(me.lastUpdated,"m/d/Y h:iA"));}});Ext.define('Ext.dataview.IndexBar',{extend:'Ext.Component',alternateClassName:'Ext.IndexBar',config:{baseCls:Ext.baseCSSPrefix+'indexbar',direction:'vertical',letters:['A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'],ui:'alphabet',listPrefix:null},itemCls:Ext.baseCSSPrefix+'',updateDirection:function(newDirection,oldDirection){var baseCls=this.getBaseCls();this.element.replaceCls(baseCls+'-'+oldDirection,baseCls+'-'+newDirection);},getElementConfig:function(){return{reference:'wrapper',classList:['x-centered','x-indexbar-wrapper'],children:[this.callParent()]};},updateLetters:function(letters){this.innerElement.setHtml('');if(letters){var ln=letters.length,i;for(i=0;i<ln;i++){this.innerElement.createChild({html:letters[i]});}}},updateListPrefix:function(listPrefix){if(listPrefix&&listPrefix.length){this.innerElement.createChild({html:listPrefix},0);}},initialize:function(){this.callParent();this.innerElement.on({touchstart:this.onTouchStart,touchend:this.onTouchEnd,touchmove:this.onTouchMove,scope:this});},onTouchStart:function(e,t){e.stopPropagation();this.innerElement.addCls(this.getBaseCls()+'-pressed');this.pageBox=this.innerElement.getPageBox();this.onTouchMove(e);},onTouchEnd:function(e,t){this.innerElement.removeCls(this.getBaseCls()+'-pressed');},onTouchMove:function(e){var point=Ext.util.Point.fromEvent(e),target,pageBox=this.pageBox;if(!pageBox){pageBox=this.pageBox=this.el.getPageBox();}
if(this.getDirection()==='vertical'){if(point.y>pageBox.bottom||point.y<pageBox.top){return;}
target=Ext.Element.fromPoint(pageBox.left+(pageBox.width/2),point.y);}
else{if(point.x>pageBox.right||point.x<pageBox.left){return;}
target=Ext.Element.fromPoint(point.x,pageBox.top+(pageBox.height/2));}
if(target){this.fireEvent('index',this,target.dom.innerHTML,target);}},destroy:function(){var me=this,elements=Array.prototype.slice.call(me.innerElement.dom.childNodes),ln=elements.length,i=0;for(;i<ln;i++){Ext.removeNode(elements[i]);}
this.callParent();}},function(){Ext.deprecateMethod(this,'isHorizontal',null,"Ext.dataview.IndexBar.isHorizontal() has been removed");Ext.deprecateMethod(this,'isVertical',null,"Ext.dataview.IndexBar.isVertical() has been removed");Ext.deprecateMethod(this,'refresh',null,"Ext.dataview.IndexBar.refresh() has been removed");Ext.deprecateProperty(this,'alphabet',null,"Ext.dataview.IndexBar.alphabet has been removed");Ext.deprecateProperty(this,'itemSelector',null,"Ext.dataview.IndexBar.itemSelector has been removed");Ext.deprecateProperty(this,'store',null,"Ext.dataview.IndexBar.store has been removed");});Ext.define('Ext.dataview.ListItemHeader',{extend:'Ext.Component',xtype:'listitemheader',config:{baseCls:Ext.baseCSSPrefix+'list-header',docked:'top'}});Ext.define('Ext.data.JsonP',{alternateClassName:'Ext.util.JSONP',singleton:true,statics:{requestCount:0,requests:{}},timeout:30000,disableCaching:true,disableCachingParam:'_dc',callbackKey:'callback',request:function(options){options=Ext.apply({},options);if(!options.url){Ext.Logger.error('A url must be specified for a JSONP request.');}
var me=this,disableCaching=Ext.isDefined(options.disableCaching)?options.disableCaching:me.disableCaching,cacheParam=options.disableCachingParam||me.disableCachingParam,id=++me.statics().requestCount,callbackName=options.callbackName||'callback'+id,callbackKey=options.callbackKey||me.callbackKey,timeout=Ext.isDefined(options.timeout)?options.timeout:me.timeout,params=Ext.apply({},options.params),url=options.url,name=Ext.isSandboxed?Ext.getUniqueGlobalNamespace():'Ext',request,script;params[callbackKey]=name+'.data.JsonP.'+callbackName;if(disableCaching){params[cacheParam]=new Date().getTime();}
script=me.createScript(url,params,options);me.statics().requests[id]=request={url:url,params:params,script:script,id:id,scope:options.scope,success:options.success,failure:options.failure,callback:options.callback,callbackKey:callbackKey,callbackName:callbackName};if(timeout>0){request.timeout=setTimeout(Ext.bind(me.handleTimeout,me,[request]),timeout);}
me.setupErrorHandling(request);me[callbackName]=Ext.bind(me.handleResponse,me,[request],true);me.loadScript(request);return request;},abort:function(request){var requests=this.statics().requests,key;if(request){if(!request.id){request=requests[request];}
this.abort(request);}else{for(key in requests){if(requests.hasOwnProperty(key)){this.abort(requests[key]);}}}},setupErrorHandling:function(request){request.script.onerror=Ext.bind(this.handleError,this,[request]);},handleAbort:function(request){request.errorType='abort';this.handleResponse(null,request);},handleError:function(request){request.errorType='error';this.handleResponse(null,request);},cleanupErrorHandling:function(request){request.script.onerror=null;},handleTimeout:function(request){request.errorType='timeout';this.handleResponse(null,request);},handleResponse:function(result,request){var success=true;if(request.timeout){clearTimeout(request.timeout);}
delete this[request.callbackName];delete this.statics()[request.id];this.cleanupErrorHandling(request);Ext.fly(request.script).destroy();if(request.errorType){success=false;Ext.callback(request.failure,request.scope,[request.errorType]);}else{Ext.callback(request.success,request.scope,[result]);}
Ext.callback(request.callback,request.scope,[success,result,request.errorType]);},createScript:function(url,params,options){var script=document.createElement('script');script.setAttribute("src",Ext.urlAppend(url,Ext.Object.toQueryString(params)));script.setAttribute("async",true);script.setAttribute("type","text/javascript");return script;},loadScript:function(request){Ext.getHead().appendChild(request.script);}});Ext.define('Ext.tab.Tab',{extend:'Ext.Button',xtype:'tab',alternateClassName:'Ext.Tab',isTab:true,config:{baseCls:Ext.baseCSSPrefix+'tab',pressedCls:Ext.baseCSSPrefix+'tab-pressed',activeCls:Ext.baseCSSPrefix+'tab-active',active:false,title:'&nbsp;'},template:[{tag:'span',reference:'badgeElement',hidden:true},{tag:'span',className:Ext.baseCSSPrefix+'button-icon',reference:'iconElement',style:'visibility: hidden !important'},{tag:'span',reference:'textElement',hidden:true}],updateTitle:function(title){this.setText(title);},hideIconElement:function(){this.iconElement.dom.style.setProperty('visibility','hidden','!important');},showIconElement:function(){this.iconElement.dom.style.setProperty('visibility','visible','!important');},updateActive:function(active,oldActive){var activeCls=this.getActiveCls();if(active&&!oldActive){this.element.addCls(activeCls);this.fireEvent('activate',this);}else if(oldActive){this.element.removeCls(activeCls);this.fireEvent('deactivate',this);}}},function(){this.override({activate:function(){this.setActive(true);},deactivate:function(){this.setActive(false);}});});Ext.define('Ext.mixin.Selectable',{extend:'Ext.mixin.Mixin',mixinConfig:{id:'selectable',hooks:{updateStore:'updateStore'}},config:{disableSelection:null,mode:'SINGLE',allowDeselect:false,lastSelected:null,lastFocused:null,deselectOnContainerClick:true},modes:{SINGLE:true,SIMPLE:true,MULTI:true},selectableEventHooks:{addrecords:'onSelectionStoreAdd',removerecords:'onSelectionStoreRemove',updaterecord:'onSelectionStoreUpdate',load:'refreshSelection',refresh:'refreshSelection'},constructor:function(){this.selected=new Ext.util.MixedCollection();this.callParent(arguments);},applyMode:function(mode){mode=mode?mode.toUpperCase():'SINGLE';return this.modes[mode]?mode:'SINGLE';},updateStore:function(newStore,oldStore){var me=this,bindEvents=Ext.apply({},me.selectableEventHooks,{scope:me});if(oldStore&&Ext.isObject(oldStore)&&oldStore.isStore){if(oldStore.autoDestroy){oldStore.destroy();}
else{oldStore.un(bindEvents);}}
if(newStore){newStore.on(bindEvents);me.refreshSelection();}},selectAll:function(silent){var me=this,selections=me.getStore().getRange(),ln=selections.length,i=0;for(;i<ln;i++){me.select(selections[i],true,silent);}},deselectAll:function(supress){var me=this,selections=me.getStore().getRange();me.deselect(selections,supress);me.selected.clear();me.setLastSelected(null);me.setLastFocused(null);},selectWithEvent:function(record){var me=this,isSelected=me.isSelected(record);switch(me.getMode()){case'MULTI':case'SIMPLE':if(isSelected){me.deselect(record);}
else{me.select(record,true);}
break;case'SINGLE':if(me.getAllowDeselect()&&isSelected){me.deselect(record);}else{me.select(record,false);}
break;}},selectRange:function(startRecord,endRecord,keepExisting,dir){var me=this,store=me.getStore(),startRow=store.indexOf(startRecord),endRow=store.indexOf(endRecord),selectedCount=0,tmp,dontDeselect,i;if(me.getDisableSelection()){return;}
if(startRow>endRow){tmp=endRow;endRow=startRow;startRow=tmp;}
for(i=startRow;i<=endRow;i++){if(me.isSelected(store.getAt(i))){selectedCount++;}}
if(!dir){dontDeselect=-1;}
else{dontDeselect=(dir=='up')?startRow:endRow;}
for(i=startRow;i<=endRow;i++){if(selectedCount==(endRow-startRow+1)){if(i!=dontDeselect){me.deselect(i,true);}}else{me.select(i,true);}}},select:function(records,keepExisting,suppressEvent){var me=this,record;if(me.getDisableSelection()){return;}
if(typeof records==="number"){records=[me.getStore().getAt(records)];}
if(!records){return;}
if(me.getMode()=="SINGLE"&&records){record=records.length?records[0]:records;me.doSingleSelect(record,suppressEvent);}else{me.doMultiSelect(records,keepExisting,suppressEvent);}},doSingleSelect:function(record,suppressEvent){var me=this,selected=me.selected;if(me.getDisableSelection()){return;}
if(me.isSelected(record)){return;}
if(selected.getCount()>0){me.deselect(me.getLastSelected(),suppressEvent);}
selected.add(record);me.setLastSelected(record);me.onItemSelect(record,suppressEvent);me.setLastFocused(record);me.fireSelectionChange(!suppressEvent);},doMultiSelect:function(records,keepExisting,suppressEvent){if(records===null||this.getDisableSelection()){return;}
records=!Ext.isArray(records)?[records]:records;var me=this,selected=me.selected,ln=records.length,change=false,i=0,record;if(!keepExisting&&selected.getCount()>0){change=true;me.deselect(me.getSelection(),true);}
for(;i<ln;i++){record=records[i];if(keepExisting&&me.isSelected(record)){continue;}
change=true;me.setLastSelected(record);selected.add(record);if(!suppressEvent){me.setLastFocused(record);}
me.onItemSelect(record,suppressEvent);}
this.fireSelectionChange(change&&!suppressEvent);},deselect:function(records,suppressEvent){var me=this;if(me.getDisableSelection()){return;}
records=Ext.isArray(records)?records:[records];var selected=me.selected,change=false,i=0,store=me.getStore(),ln=records.length,record;for(;i<ln;i++){record=records[i];if(typeof record==='number'){record=store.getAt(record);}
if(selected.remove(record)){if(me.getLastSelected()==record){me.setLastSelected(selected.last());}
change=true;}
if(record){me.onItemDeselect(record,suppressEvent);}}
me.fireSelectionChange(change&&!suppressEvent);},updateLastFocused:function(newRecord,oldRecord){this.onLastFocusChanged(oldRecord,newRecord);},fireSelectionChange:function(fireEvent){var me=this;if(fireEvent){me.fireAction('beforeselectionchange',[me],function(){me.fireEvent('selectionchange',me,me.getSelection());});}},getSelection:function(){return this.selected.getRange();},isSelected:function(record){record=Ext.isNumber(record)?this.getStore().getAt(record):record;return this.selected.indexOf(record)!==-1;},hasSelection:function(){return this.selected.getCount()>0;},refreshSelection:function(){var me=this,selections=me.getSelection();me.deselectAll(true);if(selections.length){me.select(selections,false,true);}},onSelectionStoreClear:function(){var me=this,selected=me.selected;if(selected.getCount()>0){selected.clear();me.setLastSelected(null);me.setLastFocused(null);me.fireSelectionChange(true);}},onSelectionStoreRemove:function(store,record){var me=this,selected=me.selected;if(me.getDisableSelection()){return;}
if(selected.remove(record)){if(me.getLastSelected()==record){me.setLastSelected(null);}
if(me.getLastFocused()==record){me.setLastFocused(null);}
me.fireSelectionChange(true);}},getSelectionCount:function(){return this.selected.getCount();},onSelectionStoreAdd:Ext.emptyFn,onSelectionStoreUpdate:Ext.emptyFn,onItemSelect:Ext.emptyFn,onItemDeselect:Ext.emptyFn,onLastFocusChanged:Ext.emptyFn,onEditorKey:Ext.emptyFn},function(){this.override({constructor:function(config){if(config&&config.hasOwnProperty('locked')){var locked=config.locked;config.disableSelection=locked;delete config.locked;}
this.callParent([config]);}});Ext.deprecateClassMethod(this,{isLocked:'getDisableSelection',getSelectionMode:'getMode',doDeselect:'deselect',doSelect:'select',bind:'setStore',clearSelections:'deselectAll',getCount:'getSelectionCount'});});Ext.define('Ext.dataview.element.Container',{extend:'Ext.Component',doInitialize:function(){this.element.on({touchstart:'onItemTouchStart',touchend:'onItemTouchEnd',tap:'onItemTap',taphold:'onItemTapHold',touchmove:'onItemTouchMove',singletap:'onItemSingleTap',doubletap:'onItemDoubleTap',swipe:'onItemSwipe',delegate:'> div',scope:this});},initialize:function(){this.callParent();this.doInitialize();},updateBaseCls:function(newBaseCls,oldBaseCls){var me=this;me.callParent([newBaseCls+'-container',oldBaseCls]);},onItemTouchStart:function(e){var me=this,target=e.getTarget(),index=me.getViewItems().indexOf(target);Ext.get(target).on({touchmove:'onItemTouchMove',scope:me,single:true});me.fireEvent('itemtouchstart',me,Ext.get(target),index,e);},onItemTouchEnd:function(e){var me=this,target=e.getTarget(),index=me.getViewItems().indexOf(target);Ext.get(target).un({touchmove:'onItemTouchMove',scope:me});me.fireEvent('itemtouchend',me,Ext.get(target),index,e);},onItemTouchMove:function(e){var me=this,target=e.getTarget(),index=me.getViewItems().indexOf(target);me.fireEvent('itemtouchmove',me,Ext.get(target),index,e);},onItemTap:function(e){var me=this,target=e.getTarget(),index=me.getViewItems().indexOf(target);me.fireEvent('itemtap',me,Ext.get(target),index,e);},onItemTapHold:function(e){var me=this,target=e.getTarget(),index=me.getViewItems().indexOf(target);me.fireEvent('itemtaphold',me,Ext.get(target),index,e);},onItemDoubleTap:function(e){var me=this,target=e.getTarget(),index=me.getViewItems().indexOf(target);me.fireEvent('itemdoubletap',me,Ext.get(target),index,e);},onItemSingleTap:function(e){var me=this,target=e.getTarget(),index=me.getViewItems().indexOf(target);me.fireEvent('itemsingletap',me,Ext.get(target),index,e);},onItemSwipe:function(e){var me=this,target=e.getTarget(),index=me.getViewItems().indexOf(target);me.fireEvent('itemswipe',me,Ext.get(target),index,e);},updateListItem:function(record,item){var me=this,dataview=me.dataview,data=dataview.prepareData(record.getData(true),dataview.getStore().indexOf(record),record);item.innerHTML=me.dataview.getItemTpl().apply(data);},addListItem:function(index,record){var me=this,dataview=me.dataview,data=dataview.prepareData(record.getData(true),dataview.getStore().indexOf(record),record),element=me.element,childNodes=element.dom.childNodes,ln=childNodes.length,wrapElement;wrapElement=Ext.Element.create(this.getItemElementConfig(index,data));if(!ln||index==ln){wrapElement.appendTo(element);}else{wrapElement.insertBefore(childNodes[index]);}},getItemElementConfig:function(index,data){var dataview=this.dataview,itemCls=dataview.getItemCls(),cls=dataview.getBaseCls()+'-item';if(itemCls){cls+=' '+itemCls;}
return{cls:cls,html:dataview.getItemTpl().apply(data)};},doRemoveItemCls:function(cls){var elements=this.getViewItems(),ln=elements.length,i=0;for(;i<ln;i++){Ext.fly(elements[i]).removeCls(cls);}},doAddItemCls:function(cls){var elements=this.getViewItems(),ln=elements.length,i=0;for(;i<ln;i++){Ext.fly(elements[i]).addCls(cls);}},moveItemsToCache:function(from,to){var me=this,items=me.getViewItems(),i=to-from,item;for(;i>=0;i--){item=items[from+i];item.parentNode.removeChild(item);}
if(me.getViewItems().length==0){this.dataview.showEmptyText();}},moveItemsFromCache:function(records){var me=this,dataview=me.dataview,store=dataview.getStore(),ln=records.length,i,record;if(ln){dataview.hideEmptyText();}
for(i=0;i<ln;i++){records[i]._tmpIndex=store.indexOf(records[i]);}
Ext.Array.sort(records,function(record1,record2){return record1._tmpIndex>record2._tmpIndex?1:-1;});for(i=0;i<ln;i++){record=records[i];me.addListItem(record._tmpIndex,record);delete record._tmpIndex;}},getViewItems:function(){return Array.prototype.slice.call(this.element.dom.childNodes);},destroy:function(){var elements=this.getViewItems(),ln=elements.length,i=0;for(;i<ln;i++){Ext.removeNode(elements[i]);}
this.callParent();}});Ext.define('Ext.slider.Thumb',{extend:'Ext.Component',xtype:'thumb',config:{baseCls:Ext.baseCSSPrefix+'thumb',draggable:{direction:'horizontal'}},elementWidth:0,initialize:function(){this.callParent();this.getDraggable().onBefore({dragstart:'onDragStart',drag:'onDrag',dragend:'onDragEnd',scope:this});this.on('painted','onPainted');},onDragStart:function(){if(this.isDisabled()){return false;}
this.relayEvent(arguments);},onDrag:function(){if(this.isDisabled()){return false;}
this.relayEvent(arguments);},onDragEnd:function(){if(this.isDisabled()){return false;}
this.relayEvent(arguments);},onPainted:function(){this.elementWidth=this.element.dom.offsetWidth;},getElementWidth:function(){return this.elementWidth;}});Ext.define('Ext.dataview.component.DataItem',{extend:'Ext.Container',xtype:'dataitem',config:{baseCls:Ext.baseCSSPrefix+'data-item',defaultType:'component',record:null,itemCls:null,dataMap:{},items:[{xtype:'component'}]},updateBaseCls:function(newBaseCls,oldBaseCls){var me=this;me.callParent(arguments);},updateItemCls:function(newCls,oldCls){if(oldCls){this.removeCls(oldCls);}
if(newCls){this.addCls(newCls);}},updateRecord:function(newRecord){if(!newRecord){return;}
this._record=newRecord;var me=this,dataview=me.config.dataview,data=dataview.prepareData(newRecord.getData(true),dataview.getStore().indexOf(newRecord),newRecord),items=me.getItems(),item=items.first(),dataMap=me.getDataMap(),componentName,component,setterMap,setterName;if(!item){return;}
for(componentName in dataMap){setterMap=dataMap[componentName];component=me[componentName]();if(component){for(setterName in setterMap){if(component[setterName]){component[setterName](data[setterMap[setterName]]);}}}}
me.fireEvent('updatedata',me,data);item.updateData(data);}});Ext.define('zvsMobile.view.SettingsLogIn',{extend:'Ext.Panel',requires:['Ext.field.Password','Ext.form.FieldSet'],xtype:'LogIn',constructor:function(config){var self=this;Ext.apply(config||{},{items:[{xtype:'fieldset',style:'padding:10px;',items:[{xtype:'textfield',id:'APIURL_textfield',name:'apiURL',value:(zvsMobile.app.BaseURL()===''&&window.location.origin!=undefined?window.location.origin+"/API/":zvsMobile.app.BaseURL()),label:'HTTP API URL'},{xtype:'passwordfield',id:'loginPanel_password',name:'password',label:'Password',listeners:{keyup:function(t,e){if(e.browserEvent.keyCode==13){var submitButton=Ext.getCmp('submitButton');submitButton._handler.call(submitButton.scope,submitButton,Ext.EventObject());}}}},{xtype:'button',id:'submitButton',text:'Login',width:'90%',style:'margin:10px auto;',handler:function(b){var password=Ext.getCmp('loginPanel_password');if(password.getValue()==''){password.focus();return false;}
var APIURL_textfield=Ext.getCmp('APIURL_textfield');var enteredURL=APIURL_textfield.getValue();if(enteredURL==''){APIURL_textfield.focus();return false;}
appSettingsStore=Ext.getStore('appSettingsStore');var BaseURLRecord=appSettingsStore.findRecord('SettingName','BaseURL');if(BaseURLRecord!=null){BaseURLRecord.set('Value',enteredURL);appSettingsStore.sync();}
else{appSettingsStore.add({SettingName:'BaseURL',Value:enteredURL});appSettingsStore.sync();}
zvsMobile.app.SetStoreProxys();Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/login',method:'POST',params:{u:Math.random(),password:password.getValue()},success:function(response,opts){console.log(response);var result=JSON.parse(response.responseText);if(result.success){password.blur();password.setValue('');self.fireEvent('loggedIn');}
else{Ext.Msg.alert('Invalid Credentials.','Please try again.');}},failure:function(result,request){Ext.Msg.alert('Communication Error.','Please try again.');}});}}]}]});this.callParent([config]);},config:{scrollable:'vertical',layout:'fit'}});Ext.define('zvsMobile.view.SettingsViewPort',{extend:'Ext.Panel',xtype:'SettingsViewPort',requires:['zvsMobile.view.SettingsLogIn','zvsMobile.view.SettingsLogOut'],initialize:function(){this.callParent(arguments);this.getEventDispatcher().addListener('element','#SettingsViewPort','swipe',this.onTouchPadEvent,this);},onTouchPadEvent:function(e,target,options,eventController){if(e.direction==='right'&&e.distance>50&&zvsMobile.tabPanel.getTabBar().getComponent(2)._disabled!=true){zvsMobile.tabPanel.getTabBar().getComponent(0).fireEvent('tap',zvsMobile.tabPanel.getTabBar().getComponent(2));}},constructor:function(config){var self=this;Ext.apply(config||{},{items:[{xtype:'toolbar',docked:'top',title:'Settings',items:[]},{xtype:'LogIn',listeners:{loggedIn:function(){var logoutPanel=self.items.items[2];logoutPanel.UpdateLogoutHTML();self.setActiveItem(logoutPanel);zvsMobile.tabPanel.getTabBar().getComponent(0).setDisabled(false);zvsMobile.tabPanel.getTabBar().getComponent(1).setDisabled(false);zvsMobile.tabPanel.getTabBar().getComponent(2).setDisabled(false);DeviceStore.load();SceneStore.load();GroupStore.load();zvsMobile.tabPanel.getTabBar().getComponent(0).fireEvent('tap',zvsMobile.tabPanel.getTabBar().getComponent(0));}}},{xtype:'LogOut',listeners:{loggedOut:function(){var logInPanel=self.items.items[1];self.setActiveItem(logInPanel);zvsMobile.tabPanel.getTabBar().getComponent(0).setDisabled(true);zvsMobile.tabPanel.getTabBar().getComponent(1).setDisabled(true);zvsMobile.tabPanel.getTabBar().getComponent(2).setDisabled(true);zvsMobile.tabPanel.getTabBar().getComponent(0).fireEvent('tap',zvsMobile.tabPanel.getTabBar().getComponent(3));}}}],listeners:{activate:function(){if(zvsMobile.app.BaseURL()!=''){Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/login',method:'GET',params:{u:Math.random()},success:function(response,opts){if(response.responseText!=''){var result=JSON.parse(response.responseText);if(result.success&&result.isLoggedIn){var logoutPanel=self.items.items[2];self.setActiveItem(logoutPanel);}
else{self.items.items[2].fireEvent('loggedOut');}}
else{self.items.items[2].fireEvent('loggedOut');}},failure:function(result,request){self.items.items[2].fireEvent('loggedOut');}});}}}});this.callParent([config]);},config:{layout:'card'}});Ext.define('Ext.data.proxy.JsonP',{extend:'Ext.data.proxy.Server',alternateClassName:'Ext.data.ScriptTagProxy',alias:['proxy.jsonp','proxy.scripttag'],requires:['Ext.data.JsonP'],config:{defaultWriterType:'base',callbackKey:'callback',recordParam:'records',autoAppendParams:true},doRequest:function(operation,callback,scope){var action=operation.getAction();if(action!=='read'){Ext.Logger.error('JsonP proxies can only be used to read data.');}
var me=this,request=me.buildRequest(operation),params=request.getParams();request.setConfig({callbackKey:me.getCallbackKey(),timeout:me.getTimeout(),scope:me,callback:me.createRequestCallback(request,operation,callback,scope)});if(me.getAutoAppendParams()){request.setParams({});}
request.setJsonP(Ext.data.JsonP.request(request.getCurrentConfig()));request.setParams(params);operation.setStarted();me.lastRequest=request;return request;},createRequestCallback:function(request,operation,callback,scope){var me=this;return function(success,response,errorType){delete me.lastRequest;me.processResponse(success,operation,request,response,callback,scope);};},setException:function(operation,response){operation.setException(operation.getRequest().getJsonP().errorType);},buildUrl:function(request){var me=this,url=me.callParent(arguments),params=Ext.apply({},request.getParams()),filters=params.filters,records,filter,i,value;delete params.filters;if(me.getAutoAppendParams()){url=Ext.urlAppend(url,Ext.Object.toQueryString(params));}
if(filters&&filters.length){for(i=0;i<filters.length;i++){filter=filters[i];value=filter.getValue();if(value){url=Ext.urlAppend(url,filter.getProperty()+"="+value);}}}
return url;},destroy:function(){this.abort();this.callParent(arguments);},abort:function(){var lastRequest=this.lastRequest;if(lastRequest){Ext.data.JsonP.abort(lastRequest.getJsonP());}}});Ext.define('Ext.tab.Bar',{extend:'Ext.Toolbar',alternateClassName:'Ext.TabBar',xtype:'tabbar',requires:['Ext.tab.Tab'],config:{baseCls:Ext.baseCSSPrefix+'tabbar',defaultType:'tab',layout:{type:'hbox',align:'middle'}},eventedConfig:{activeTab:null},initialize:function(){var me=this;me.callParent();me.on({tap:'onTabTap',delegate:'> tab',scope:me});},onTabTap:function(tab){this.setActiveTab(tab);},applyActiveTab:function(activeTab,oldActiveTab){if(!activeTab&&activeTab!==0){return;}
var activeTabInstance=this.parseActiveTab(activeTab);if(!activeTabInstance){if(oldActiveTab){Ext.Logger.warn('Trying to set a non-existent activeTab');}
return;}
return activeTabInstance;},doSetDocked:function(newDocked){var layout=this.getLayout(),pack=newDocked=='bottom'?'center':'left';if(layout.isLayout){layout.setPack(pack);}else{layout.pack=(layout&&layout.pack)?layout.pack:pack;}},doSetActiveTab:function(newTab,oldTab){if(newTab){newTab.setActive(true);}
if(oldTab){oldTab.setActive(false);}},parseActiveTab:function(tab){if(typeof tab=='number'){return this.getInnerItems()[tab];}
else if(typeof tab=='string'){tab=Ext.getCmp(tab);}
return tab;}});Ext.define('Ext.tab.Panel',{extend:'Ext.Container',xtype:'tabpanel',alternateClassName:'Ext.TabPanel',requires:['Ext.tab.Bar'],config:{ui:'dark',tabBar:true,tabBarPosition:'top',layout:{type:'card',animation:{type:'slide',direction:'left'}},cls:Ext.baseCSSPrefix+'tabpanel'},delegateListeners:{delegate:'> component',centeredchange:'onItemCenteredChange',dockedchange:'onItemDockedChange',floatingchange:'onItemFloatingChange',disabledchange:'onItemDisabledChange'},initialize:function(){this.callParent();this.on({order:'before',activetabchange:'doTabChange',delegate:'> tabbar',scope:this});var layout=this.getLayout();if(layout&&!layout.isCard){Ext.Logger.error('The base layout for a TabPanel must always be a Card Layout');}},applyScrollable:function(){return false;},updateUi:function(newUi,oldUi){this.callParent(arguments);if(this.initialized){this.getTabBar().setUi(newUi);}},doSetActiveItem:function(newActiveItem,oldActiveItem){if(newActiveItem){var items=this.getInnerItems(),oldIndex=items.indexOf(oldActiveItem),newIndex=items.indexOf(newActiveItem),reverse=oldIndex>newIndex,animation=this.getLayout().getAnimation(),tabBar=this.getTabBar(),oldTab=tabBar.parseActiveTab(oldIndex),newTab=tabBar.parseActiveTab(newIndex);if(animation&&animation.setReverse){animation.setReverse(reverse);}
this.callParent(arguments);if(newIndex!=-1){this.getTabBar().setActiveTab(newIndex);if(oldTab){oldTab.setActive(false);}
if(newTab){newTab.setActive(true);}}}},doTabChange:function(tabBar,newTab){this.setActiveItem(tabBar.indexOf(newTab));},applyTabBar:function(config){if(config===true){config={};}
if(config){Ext.applyIf(config,{ui:this.getUi(),docked:this.getTabBarPosition()});}
return Ext.factory(config,Ext.tab.Bar,this.getTabBar());},updateTabBar:function(newTabBar){if(newTabBar){this.add(newTabBar);this.setTabBarPosition(newTabBar.getDocked());}},updateTabBarPosition:function(position){var tabBar=this.getTabBar();if(tabBar){tabBar.setDocked(position);}},onItemAdd:function(card){var me=this;if(!card.isInnerItem()){return me.callParent(arguments);}
var tabBar=me.getTabBar(),initialConfig=card.getInitialConfig(),tabConfig=initialConfig.tab||{},tabTitle=initialConfig.title,tabIconCls=initialConfig.iconCls,tabHidden=initialConfig.hidden,tabDisabled=initialConfig.disabled,tabBadgeText=initialConfig.badgeText,innerItems=me.getInnerItems(),index=innerItems.indexOf(card),tabs=tabBar.getItems(),cards=me.getInnerItems(),currentTabInstance=(tabs.length>=cards.length)&&tabs.getAt(index),tabInstance;if(tabTitle&&!tabConfig.title){tabConfig.title=tabTitle;}
if(tabIconCls&&!tabConfig.iconCls){tabConfig.iconCls=tabIconCls;}
if(tabHidden&&!tabConfig.hidden){tabConfig.hidden=tabHidden;}
if(tabDisabled&&!tabConfig.disabled){tabConfig.disabled=tabDisabled;}
if(tabBadgeText&&!tabConfig.badgeText){tabConfig.badgeText=tabBadgeText;}
if(!currentTabInstance&&!tabConfig.title&&!tabConfig.iconCls){if(!tabConfig.title&&!tabConfig.iconCls){Ext.Logger.error('Adding a card to a tab container without specifying any tab configuration');}}
tabInstance=Ext.factory(tabConfig,Ext.tab.Tab,currentTabInstance);if(!currentTabInstance){tabBar.insert(index,tabInstance);}
card.tab=tabInstance;me.callParent(arguments);},onItemDisabledChange:function(item,newDisabled){if(item&&item.tab){item.tab.setDisabled(newDisabled);}},onItemRemove:function(item,index){this.getTabBar().remove(item.tab,this.getAutoDestroy());this.callParent(arguments);}},function(){Ext.deprecateProperty(this,'tabBarDock','tabBarPosition');});Ext.define('zvsMobile.view.Main',{extend:'Ext.tab.Panel',config:{fullscreen:true}});Ext.define('zvsMobile.view.phone.Main',{extend:'zvsMobile.view.Main',xtype:'mainview',config:{fullscreen:true,tabBar:{docked:'bottom',layout:{pack:'center'}},items:[{xtype:'DevicePhoneViewPort',id:'DevicePhoneViewPort',title:'Devices',iconCls:"bulb"},{xtype:'ScenePhoneViewPort',id:'ScenePhoneViewPort',title:"Scenes",iconCls:"equalizer2"},{xtype:'GroupPhoneViewPort',id:'GroupPhoneViewPort',title:"Groups",iconCls:"spaces2"},{xtype:'SettingsViewPort',id:'SettingsViewPort',title:'Settings',iconCls:'settings'}]}});Ext.define('zvsMobile.view.tablet.Main',{extend:'zvsMobile.view.Main',xtype:'mainview',config:{fullscreen:true,tabBar:{docked:'bottom',layout:{pack:'center'}},items:[{xtype:'DeviceTabletViewPort',id:'DeviceTabletViewPort',title:'Devices',iconCls:"bulb"},{xtype:'SceneTabletViewPort',id:'SceneTabletViewPort',title:"Scenes",iconCls:"equalizer2"},{xtype:'GroupTabletViewPort',id:'GroupTabletViewPort',title:"Groups",iconCls:"spaces2"},{xtype:'SettingsViewPort',id:'SettingsViewPort',title:'Settings',iconCls:'settings'}]}});Ext.define('Ext.dataview.element.List',{extend:'Ext.dataview.element.Container',updateBaseCls:function(newBaseCls){var me=this;me.itemClsShortCache=newBaseCls+'-item';me.headerClsShortCache=newBaseCls+'-header';me.headerClsCache='.'+me.headerClsShortCache;me.headerItemClsShortCache=newBaseCls+'-header-item';me.footerClsShortCache=newBaseCls+'-footer-item';me.footerClsCache='.'+me.footerClsShortCache;me.labelClsShortCache=newBaseCls+'-item-label';me.labelClsCache='.'+me.labelClsShortCache;me.disclosureClsShortCache=newBaseCls+'-disclosure';me.disclosureClsCache='.'+me.disclosureClsShortCache;me.iconClsShortCache=newBaseCls+'-icon';me.iconClsCache='.'+me.iconClsShortCache;this.callParent(arguments);},hiddenDisplayCache:Ext.baseCSSPrefix+'hidden-display',getItemElementConfig:function(index,data){var me=this,dataview=me.dataview,itemCls=dataview.getItemCls(),cls=me.itemClsShortCache,config,iconSrc;if(itemCls){cls+=' '+itemCls;}
config={cls:cls,children:[{cls:me.labelClsShortCache,html:dataview.getItemTpl().apply(data)}]};if(dataview.getIcon()){iconSrc=data.iconSrc;config.children.push({cls:me.iconClsShortCache,style:'background-image: '+iconSrc?'url("'+newSrc+'")':''});}
if(dataview.getOnItemDisclosure()){config.children.push({cls:me.disclosureClsShortCache+((data[dataview.getDisclosureProperty()]===false)?me.hiddenDisplayCache:'')});}
return config;},updateListItem:function(record,item){var me=this,dataview=me.dataview,extItem=Ext.fly(item),innerItem=extItem.down(me.labelClsCache,true),data=record.data,disclosureProperty=dataview.getDisclosureProperty(),disclosure=data&&data.hasOwnProperty(disclosureProperty),iconSrc=data&&data.hasOwnProperty('iconSrc'),disclosureEl,iconEl;innerItem.innerHTML=dataview.getItemTpl().apply(data);if(disclosure&&data[disclosureProperty]===false){disclosureEl=extItem.down(me.disclosureClsCache);disclosureEl[disclosure?'removeCls':'addCls'](me.hiddenDisplayCache);}
if(dataview.getIcon()){iconEl=extItem.down(me.iconClsCache,true);iconEl.style.backgroundImage=iconSrc?'url("'+iconSrc+'")':'';}},doRemoveHeaders:function(){var me=this,headerClsShortCache=me.headerItemClsShortCache,existingHeaders=me.element.query(me.headerClsCache),existingHeadersLn=existingHeaders.length,i=0,item;for(;i<existingHeadersLn;i++){item=existingHeaders[i];Ext.fly(item.parentNode).removeCls(headerClsShortCache);Ext.removeNode(item);}},doRemoveFooterCls:function(){var me=this,footerClsShortCache=me.footerClsShortCache,existingFooters=me.element.query(me.footerClsCache),existingFootersLn=existingFooters.length,i=0;for(;i<existingFootersLn;i++){Ext.fly(existingFooters[i]).removeCls(footerClsShortCache);}},doAddHeader:function(item,html){item=Ext.fly(item);if(html){item.insertFirst(Ext.Element.create({cls:this.headerClsShortCache,html:html}));}
item.addCls(this.headerItemClsShortCache);},destroy:function(){this.doRemoveHeaders();this.callParent();}});Ext.define('Ext.slider.Slider',{extend:'Ext.Container',xtype:'slider',requires:['Ext.slider.Thumb','Ext.fx.easing.EaseOut'],config:{baseCls:'x-slider',thumbConfig:{draggable:{translatable:{easingX:{duration:300,type:'ease-out'}}}},value:0,minValue:0,maxValue:100,increment:1,allowThumbsOverlapping:false,animation:true},elementWidth:0,offsetValueRatio:0,activeThumb:null,constructor:function(config){config=config||{};if(config.hasOwnProperty('values')){config.value=config.values;}
this.callParent([config]);},initialize:function(){var element=this.element;this.callParent();element.on({scope:this,tap:'onTap'});this.on({scope:this,delegate:'> thumb',dragstart:'onThumbDragStart',drag:'onThumbDrag',dragend:'onThumbDragEnd'});this.on({painted:'refresh',resize:'refresh'});},factoryThumb:function(){return Ext.factory(this.getThumbConfig(),Ext.slider.Thumb);},getThumbs:function(){return this.innerItems;},getThumb:function(index){if(typeof index!='number'){index=0;}
return this.innerItems[index];},refreshOffsetValueRatio:function(){var valueRange=this.getMaxValue()-this.getMinValue(),trackWidth=this.elementWidth-this.thumbWidth;this.offsetValueRatio=trackWidth/valueRange;},refreshElementWidth:function(){this.elementWidth=this.element.dom.offsetWidth;var thumb=this.getThumb(0);if(thumb){this.thumbWidth=thumb.getElementWidth();}},refresh:function(){this.refreshElementWidth();this.refreshValue();},setActiveThumb:function(thumb){var oldActiveThumb=this.activeThumb;if(oldActiveThumb&&oldActiveThumb!==thumb){oldActiveThumb.setZIndex(null);}
this.activeThumb=thumb;thumb.setZIndex(2);return this;},onThumbDragStart:function(thumb,e){if(e.absDeltaX<=e.absDeltaY){return false;}
else{e.stopPropagation();}
if(this.getAllowThumbsOverlapping()){this.setActiveThumb(thumb);}
this.dragStartValue=this.getValue()[this.getThumbIndex(thumb)];this.fireEvent('dragstart',this,thumb,this.dragStartValue,e);},onThumbDrag:function(thumb,e,offsetX){var index=this.getThumbIndex(thumb),offsetValueRatio=this.offsetValueRatio,constrainedValue=this.constrainValue(offsetX/offsetValueRatio);e.stopPropagation();this.setIndexValue(index,constrainedValue);this.fireEvent('drag',this,thumb,this.getValue(),e);return false;},setIndexValue:function(index,value,animation){var thumb=this.getThumb(index),values=this.getValue(),offsetValueRatio=this.offsetValueRatio,draggable=thumb.getDraggable();draggable.setOffset(value*offsetValueRatio,null,animation);values[index]=this.constrainValue(draggable.getOffset().x/offsetValueRatio);},onThumbDragEnd:function(thumb,e){this.refreshThumbConstraints(thumb);var index=this.getThumbIndex(thumb),newValue=this.getValue()[index],oldValue=this.dragStartValue;this.fireEvent('dragend',this,thumb,this.getValue(),e);if(oldValue!==newValue){this.fireEvent('change',this,thumb,newValue,oldValue);}},getThumbIndex:function(thumb){return this.getThumbs().indexOf(thumb);},refreshThumbConstraints:function(thumb){var allowThumbsOverlapping=this.getAllowThumbsOverlapping(),offsetX=thumb.getDraggable().getOffset().x,thumbs=this.getThumbs(),index=this.getThumbIndex(thumb),previousThumb=thumbs[index-1],nextThumb=thumbs[index+1],thumbWidth=this.thumbWidth;if(previousThumb){previousThumb.getDraggable().addExtraConstraint({max:{x:offsetX-((allowThumbsOverlapping)?0:thumbWidth)}});}
if(nextThumb){nextThumb.getDraggable().addExtraConstraint({min:{x:offsetX+((allowThumbsOverlapping)?0:thumbWidth)}});}},onTap:function(e){if(this.isDisabled()){return;}
var targetElement=Ext.get(e.target);if(!targetElement||targetElement.hasCls('x-thumb')){return;}
var touchPointX=e.touch.point.x,element=this.element,elementX=element.getX(),offset=touchPointX-elementX-(this.thumbWidth/2),value=this.constrainValue(offset/this.offsetValueRatio),values=this.getValue(),minDistance=Infinity,ln=values.length,i,absDistance,testValue,closestIndex,oldValue,thumb;if(ln===1){closestIndex=0;}
else{for(i=0;i<ln;i++){testValue=values[i];absDistance=Math.abs(testValue-value);if(absDistance<minDistance){minDistance=absDistance;closestIndex=i;}}}
oldValue=values[closestIndex];thumb=this.getThumb(closestIndex);this.setIndexValue(closestIndex,value,this.getAnimation());this.refreshThumbConstraints(thumb);if(oldValue!==value){this.fireEvent('change',this,thumb,value,oldValue);}},updateThumbs:function(newThumbs){this.add(newThumbs);},applyValue:function(value){var values=Ext.Array.from(value||0),filteredValues=[],previousFilteredValue=this.getMinValue(),filteredValue,i,ln;for(i=0,ln=values.length;i<ln;i++){filteredValue=this.constrainValue(values[i]);if(filteredValue<previousFilteredValue){Ext.Logger.warn("Invalid values of '"+Ext.encode(values)+"', values at smaller indexes must "+"be smaller than or equal to values at greater indexes");filteredValue=previousFilteredValue;}
filteredValues.push(filteredValue);previousFilteredValue=filteredValue;}
return filteredValues;},updateValue:function(newValue,oldValue){var thumbs=this.getThumbs(),ln=newValue.length,i;this.setThumbsCount(ln);for(i=0;i<ln;i++){thumbs[i].getDraggable().setExtraConstraint(null).setOffset(newValue[i]*this.offsetValueRatio);}
for(i=0;i<ln;i++){this.refreshThumbConstraints(thumbs[i]);}},refreshValue:function(){this.refreshOffsetValueRatio();this.setValue(this.getValue());},constrainValue:function(value){var me=this,minValue=me.getMinValue(),maxValue=me.getMaxValue(),increment=me.getIncrement(),remainder;value=parseFloat(value);if(isNaN(value)){value=minValue;}
remainder=value%increment;value-=remainder;if(Math.abs(remainder)>=(increment/2)){value+=(remainder>0)?increment:-increment;}
value=Math.max(minValue,value);value=Math.min(maxValue,value);return value;},setThumbsCount:function(count){var thumbs=this.getThumbs(),thumbsCount=thumbs.length,i,ln,thumb;if(thumbsCount>count){for(i=0,ln=thumbsCount-count;i<ln;i++){thumb=thumbs[thumbs.length-1];thumb.destroy();}}
else if(thumbsCount<count){for(i=0,ln=count-thumbsCount;i<ln;i++){this.add(this.factoryThumb());}}
return this;},setValues:function(value){this.setValue(value);},getValues:function(){return this.getValue();},applyIncrement:function(increment){if(increment===0){increment=1;}
return Math.abs(increment);},updateAllowThumbsOverlapping:function(newValue,oldValue){if(typeof oldValue!='undefined'){this.refreshValue();}},updateMinValue:function(newValue,oldValue){if(typeof oldValue!='undefined'){this.refreshValue();}},updateMaxValue:function(newValue,oldValue){if(typeof oldValue!='undefined'){this.refreshValue();}},updateIncrement:function(newValue,oldValue){if(typeof oldValue!='undefined'){this.refreshValue();}},doSetDisabled:function(disabled){this.callParent(arguments);var items=this.getItems().items,ln=items.length,i;for(i=0;i<ln;i++){items[i].setDisabled(disabled);}}},function(){Ext.deprecateProperty(this,'animationDuration',null,"Ext.slider.Slider.animationDuration has been removed");});Ext.define('Ext.field.Slider',{extend:'Ext.field.Field',xtype:'sliderfield',requires:['Ext.slider.Slider'],alternateClassName:'Ext.form.Slider',config:{cls:Ext.baseCSSPrefix+'slider-field',tabIndex:-1},proxyConfig:{value:0,minValue:0,maxValue:100,increment:1},constructor:function(config){config=config||{};if(config.hasOwnProperty('values')){config.value=config.values;}
this.callParent([config]);},initialize:function(){this.callParent();this.getComponent().on({scope:this,change:'onSliderChange'});},applyComponent:function(config){return Ext.factory(config,Ext.slider.Slider);},onSliderChange:function(me,thumb,newValue,oldValue){this.fireEvent('change',this,thumb,newValue,oldValue);},setValues:function(value){this.setValue(value);},getValues:function(){return this.getValue();},reset:function(){var config=this.config,initialValue=(this.config.hasOwnProperty('values'))?config.values:config.value;this.setValue(initialValue);},doSetDisabled:function(disabled){this.callParent(arguments);this.getComponent().setDisabled(disabled);}});Ext.define('zvsMobile.view.DeviceDetailsDimmer',{extend:'Ext.Panel',requires:['Ext.field.Slider','Ext.data.proxy.JsonP'],xtype:'DeviceDetailsDimmer',constructor:function(config){var self=this;self.RepollTimer;self.deviceID=0;Ext.apply(config||{},{xtype:'panel',layout:'vbox',scrollable:'vertical',items:[{xtype:'panel',id:'dimmerDetailsTPL',tpl:new Ext.XTemplate('<div class="device_info">','<div id="level_dimmer_img" class="imageholder {type}_{on_off}"></div>','<div id="level_dimmer_details" class="level">{level_txt}</div>','<h1>{name}</h1>','<h2>{type_txt}<h2>','<div class="overview"><strong>Groups: </strong>{groups}<br />','<strong>Updated: </strong>{last_heard_from}</div>','</div>')},{xtype:'fieldset',margin:5,defaults:{labelAlign:'right'},items:[{xtype:'sliderfield',id:'dimmerSlider',label:'Level',minValue:0,maxValue:99,listeners:{scope:this,change:function(slider,value){var dimmerSlider=Ext.getCmp('dimmerSlider');if(dimmerSlider){dimmerSlider.element.dom.childNodes[0].childNodes[0].innerHTML=slider.getValue()+"%";var dimmerSlider=Ext.getCmp('dimmerSlider');var sliderValue=dimmerSlider.getValue()[0]
console.log('AJAX: SendCmd SEt LEVEL'+sliderValue);Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/device/'+self.deviceID+'/command/',method:'POST',params:{u:Math.random(),name:'DYNAMIC_CMD_BASIC',arg:sliderValue,type:'device'},success:function(response,opts){var result=JSON.parse(response.responseText);if(result.success){self.delayedReload();}
else{Ext.Msg.alert('Dimmer Command','Communication Error!');}}});}}}}]},{xtype:'button',label:'Repoll',text:'Repoll',ui:'confirm',margin:'15 10 5 10',handler:function(){console.log('AJAX: SendCmd REPOLL_ME');Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/commands/',method:'POST',params:{u:Math.random(),name:'REPOLL_ME',arg:self.deviceID},success:function(response,opts){var result=JSON.parse(response.responseText);if(result.success){self.delayedReload();}
else{console.log('ERROR');}}});}}],listeners:{scope:this,deactivate:function(){if(self.RepollTimer){clearInterval(self.RepollTimer);}}}});this.callOverridden([config]);},delayedReload:function(){var self=this;var detailsTPL=self.items.items[1];if(self.RepollTimer){clearInterval(self.RepollTimer);}
self.RepollTimer=setTimeout(function(){self.loadDevice(self.deviceID);},5000);},ShowBackButton:function(){var self=this;self.items.items[0].visibility=false;},loadDevice:function(deviceId){var self=this;var detailsTPL=Ext.getCmp('dimmerDetailsTPL');self.deviceID=deviceId;console.log('AJAX: GetDeviceDetails');Ext.data.JsonP.request({url:zvsMobile.app.BaseURL()+'/device/'+deviceId,callbackKey:'callback',params:{u:Math.random()},success:function(result){detailsTPL.setData(result.details);self.UpdateLevel(result.details.level);}});},UpdateLevel:function(value){var level=value>99?99:value;var dimmerSlider=Ext.getCmp('dimmerSlider');var detailsTPL=Ext.getCmp('dimmerDetailsTPL');dimmerSlider.setValue(level);dimmerSlider.element.dom.childNodes[0].childNodes[0].innerHTML=level+"%";var data=Ext.clone(detailsTPL.getData());data.level=level;data.level_txt=level+'%';if(level==0){data.on_off='OFF';}
else if(level>98){data.on_off='ON';}
else{data.on_off='DIM';}
detailsTPL.setData(data);data=DeviceStore.data.items;for(i=0,len=data.length;i<len;i++){if(data[i].data.id===detailsTPL._data.id){data[i].data.level_txt=value+'%';data[i].data.level=value;if(value==0){data[i].data.on_off='OFF';}
else if(value>98){data[i].data.on_off='ON';}
else{data[i].data.on_off='DIM';}}}
DeviceStore.add(data);Ext.getCmp('DeviceList').refresh();}});Ext.define('Ext.slider.Toggle',{extend:'Ext.slider.Slider',config:{baseCls:'x-toggle',minValueCls:'x-toggle-off',maxValueCls:'x-toggle-on'},initialize:function(){this.callParent();this.on({change:'onChange'});},applyMinValue:function(){return 0;},applyMaxValue:function(){return 1;},applyIncrement:function(){return 1;},setValue:function(newValue,oldValue){this.callParent(arguments);this.onChange(this,this.getThumbs()[0],newValue,oldValue);},onChange:function(me,thumb,newValue,oldValue){var isOn=newValue>0,onCls=me.getMaxValueCls(),offCls=me.getMinValueCls();this.element.addCls(isOn?onCls:offCls);this.element.removeCls(isOn?offCls:onCls);}});Ext.define('Ext.field.Toggle',{extend:'Ext.field.Slider',xtype:'togglefield',alternateClassName:'Ext.form.Toggle',requires:['Ext.slider.Toggle'],config:{cls:'x-toggle-field'},proxyConfig:{minValueCls:'x-toggle-off',maxValueCls:'x-toggle-on'},applyComponent:function(config){return Ext.factory(config,Ext.slider.Toggle);},setValue:function(newValue){if(newValue===true){newValue=1;}
this.getComponent().setValue(newValue);return this;},toggle:function(){var value=this.getValue();this.setValue((value==1)?0:1);return this;},getValue:function(){return this.callParent()[0];}});Ext.define('zvsMobile.view.DeviceDetailsSwitch',{extend:'Ext.Panel',requires:['Ext.field.Toggle','Ext.data.proxy.JsonP'],xtype:'DeviceDetailsSwitch',constructor:function(config){this.RepollTimer;this.deviceID=0;var self=this;Ext.apply(config||{},{xtype:'panel',layout:'vbox',scrollable:'vertical',items:[{xtype:'panel',id:'switchDetailsTPL',tpl:new Ext.XTemplate('<div class="device_info">','<div id="level_switch_img" class="imageholder {type}_{on_off}"></div>','<div id="level_switch_details" class="level">{level_txt}</div>','<h1>{name}</h1>','<h2>{type_txt}<h2>','<div class="overview"><strong>Groups: </strong>{groups}<br />','<strong>Updated: </strong>{last_heard_from}</div>','</div>')},{xtype:'fieldset',margin:5,defaults:{labelAlign:'right'},items:[{xtype:'togglefield',id:'switchToggle',label:'OFF / ON',listeners:{scope:this,change:function(slider,value){var switchToggle=Ext.getCmp('switchToggle');if(switchToggle){var toggleValue=switchToggle.getValue();console.log('AJAX: SendCmd SEt LEVEL'+toggleValue);Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/device/'+self.deviceID+'/command/',method:'POST',params:{u:Math.random(),name:toggleValue>0?'TURNON':'TURNOFF',arg:0,type:'device_type'},success:function(response,opts){var result=JSON.parse(response.responseText);if(result.success){self.delayedReload();}
else{Ext.Msg.alert('Switch Command','Communication Error!');}}});}}}}]},{xtype:'button',label:'Repoll',text:'Repoll',ui:'confirm',margin:'15 10 5 10',handler:function(){console.log('AJAX: SendCmd REPOLL_ME');Ext.Ajax.request({url:zvsMobile.app.BaseURL()+'/commands/',method:'POST',params:{u:Math.random(),name:'REPOLL_ME',arg:self.deviceID},success:function(response,opts){var result=JSON.parse(response.responseText);if(result.success){self.delayedReload();}
else{console.log('ERROR');}}});}}],listeners:{scope:this,deactivate:function(){if(self.RepollTimer){clearInterval(self.RepollTimer);}}}});this.callOverridden([config]);},delayedReload:function(){var self=this;var switchDetailsTPL=Ext.getCmp('switchDetailsTPL');if(self.RepollTimer){clearInterval(self.RepollTimer);}
self.RepollTimer=setTimeout(function(){self.loadDevice(self.deviceID);},1500);},loadDevice:function(deviceId){var self=this;var switchDetailsTPL=Ext.getCmp('switchDetailsTPL');this.deviceID=deviceId;console.log('AJAX: GetDeviceDetails');Ext.data.JsonP.request({url:zvsMobile.app.BaseURL()+'/device/'+deviceId,callbackKey:'callback',params:{u:Math.random()},success:function(result){switchDetailsTPL.setData(result.details);self.UpdateLevel(result.details.level);}});},UpdateLevel:function(value){var self=this;var switchToggle=Ext.getCmp('switchToggle');var switchDetailsTPL=Ext.getCmp('switchDetailsTPL');switchToggle.setValue(value);var data=Ext.clone(switchDetailsTPL.getData());data.level=value;data.level_txt=value>0?'ON':'OFF';data.on_off=value>0?'ON':'OFF';switchDetailsTPL.setData(data);data=DeviceStore.data.items;for(i=0,len=data.length;i<len;i++){if(data[i].data.id===switchDetailsTPL._data.id){data[i].data.level=value;data[i].data.level_txt=value>0?'On':'Off';data[i].data.on_off=value>0?'ON':'OFF';}}
DeviceStore.add(data);Ext.getCmp('DeviceList').refresh();}});Ext.define('Ext.dataview.component.Container',{extend:'Ext.Container',requires:['Ext.dataview.component.DataItem'],constructor:function(){this.itemCache=[];this.callParent(arguments);},doInitialize:function(){this.innerElement.on({touchstart:'onItemTouchStart',touchend:'onItemTouchEnd',tap:'onItemTap',touchmove:'onItemTouchMove',singletap:'onItemSingleTap',doubletap:'onItemDoubleTap',swipe:'onItemSwipe',delegate:'> .'+Ext.baseCSSPrefix+'data-item',scope:this});},initialize:function(){this.callParent();this.doInitialize();},onItemTouchStart:function(e){var me=this,target=e.getTarget(),item=Ext.getCmp(target.id);item.on({touchmove:'onItemTouchMove',scope:me,single:true});me.fireEvent('itemtouchstart',me,item,me.indexOf(item),e);},onItemTouchMove:function(e){var me=this,target=e.getTarget(),item=Ext.getCmp(target.id);me.fireEvent('itemtouchmove',me,item,me.indexOf(item),e);},onItemTouchEnd:function(e){var me=this,target=e.getTarget(),item=Ext.getCmp(target.id);item.un({touchmove:'onItemTouchMove',scope:me});me.fireEvent('itemtouchend',me,item,me.indexOf(item),e);},onItemTap:function(e){var me=this,target=e.getTarget(),item=Ext.getCmp(target.id);me.fireEvent('itemtap',me,item,me.indexOf(item),e);},onItemTapHold:function(e){var me=this,target=e.getTarget(),item=Ext.getCmp(target.id);me.fireEvent('itemtaphold',me,item,me.indexOf(item),e);},onItemSingleTap:function(e){var me=this,target=e.getTarget(),item=Ext.getCmp(target.id);me.fireEvent('itemsingletap',me,item,me.indexOf(item),e);},onItemDoubleTap:function(e){var me=this,target=e.getTarget(),item=Ext.getCmp(target.id);me.fireEvent('itemdoubletap',me,item,me.indexOf(item),e);},onItemSwipe:function(e){var me=this,target=e.getTarget(),item=Ext.getCmp(target.id);me.fireEvent('itemswipe',me,item,me.indexOf(item),e);},moveItemsToCache:function(from,to){var me=this,dataview=me.dataview,maxItemCache=dataview.getMaxItemCache(),items=me.getViewItems(),itemCache=me.itemCache,cacheLn=itemCache.length,pressedCls=dataview.getPressedCls(),selectedCls=dataview.getSelectedCls(),i=to-from,item;for(;i>=0;i--){item=items[from+i];if(cacheLn!==maxItemCache){me.remove(item,false);item.removeCls([pressedCls,selectedCls]);itemCache.push(item);cacheLn++;}
else{item.destroy();}}
if(me.getViewItems().length==0){this.dataview.showEmptyText();}},moveItemsFromCache:function(records){var me=this,dataview=me.dataview,store=dataview.getStore(),ln=records.length,xtype=dataview.getDefaultType(),itemConfig=dataview.getItemConfig(),itemCache=me.itemCache,cacheLn=itemCache.length,items=[],i,item,record;if(ln){dataview.hideEmptyText();}
for(i=0;i<ln;i++){records[i]._tmpIndex=store.indexOf(records[i]);}
Ext.Array.sort(records,function(record1,record2){return record1._tmpIndex>record2._tmpIndex?1:-1;});for(i=0;i<ln;i++){record=records[i];if(cacheLn){cacheLn--;item=itemCache.pop();this.updateListItem(record,item);}
else{item=me.getDataItemConfig(xtype,record,itemConfig);}
this.insert(record._tmpIndex,item);delete record._tmpIndex;}
return items;},getViewItems:function(){return this.getInnerItems();},updateListItem:function(record,item){if(item.updateRecord){item.updateRecord(record);}},getDataItemConfig:function(xtype,record,itemConfig){var dataview=this.dataview,dataItemConfig={xtype:xtype,record:record,dataview:dataview,itemCls:dataview.getItemCls(),defaults:itemConfig};return Ext.merge(dataItemConfig,itemConfig);},doRemoveItemCls:function(cls){var items=this.getViewItems(),ln=items.length,i=0;for(;i<ln;i++){items[i].removeCls(cls);}},doAddItemCls:function(cls){var items=this.getViewItems(),ln=items.length,i=0;for(;i<ln;i++){items[i].addCls(cls);}},destroy:function(){var me=this,itemCache=me.itemCache,ln=itemCache.length,i=0;for(;i<ln;i++){itemCache[i].destroy();}
this.callParent();}});Ext.define('Ext.dataview.DataView',{extend:'Ext.Container',alternateClassName:'Ext.DataView',mixins:['Ext.mixin.Selectable'],xtype:'dataview',requires:['Ext.LoadMask','Ext.data.StoreManager','Ext.dataview.component.Container','Ext.dataview.element.Container'],config:{store:null,baseCls:Ext.baseCSSPrefix+'dataview',emptyText:null,deferEmptyText:true,itemTpl:'<div>{text}</div>',pressedCls:'x-item-pressed',itemCls:null,selectedCls:'x-item-selected',triggerEvent:'itemtap',triggerCtEvent:'tap',deselectOnContainerClick:true,scrollable:true,inline:null,pressedDelay:100,loadingText:'Loading...',useComponents:null,itemConfig:{},maxItemCache:20,defaultType:'dataitem',scrollToTopOnRefresh:true},constructor:function(config){var me=this;if(config&&config.layout){Ext.Logger.warn('Attempting to create a DataView with a layout. DataViews do not have a layout configuration as their items are laid out automatically.');delete config.layout;}
me.hasLoadedStore=false;me.mixins.selectable.constructor.apply(me,arguments);me.callParent(arguments);},updateItemCls:function(newCls,oldCls){var container=this.container;if(container){if(oldCls){container.doRemoveItemCls(oldCls);}
if(newCls){container.doAddItemCls(newCls);}}},storeEventHooks:{beforeload:'onBeforeLoad',load:'onLoad',refresh:'refresh',addrecords:'onStoreAdd',removerecords:'onStoreRemove',updaterecord:'onStoreUpdate'},initialize:function(){this.callParent();var me=this,container;me.on(me.getTriggerCtEvent(),me.onContainerTrigger,me);container=me.container=this.add(new Ext.dataview[me.getUseComponents()?'component':'element'].Container({baseCls:this.getBaseCls()}));container.dataview=me;me.on(me.getTriggerEvent(),me.onItemTrigger,me);container.on({itemtouchstart:'onItemTouchStart',itemtouchend:'onItemTouchEnd',itemtap:'onItemTap',itemtaphold:'onItemTapHold',itemtouchmove:'onItemTouchMove',itemsingletap:'onItemSingleTap',itemdoubletap:'onItemDoubleTap',itemswipe:'onItemSwipe',scope:me});if(this.getStore()){this.refresh();}},applyInline:function(config){if(Ext.isObject(config)){config=Ext.apply({},config);}
return config;},updateInline:function(newInline,oldInline){var baseCls=this.getBaseCls();if(oldInline){this.removeCls([baseCls+'-inlineblock',baseCls+'-nowrap']);}
if(newInline){this.addCls(baseCls+'-inlineblock');if(Ext.isObject(newInline)&&newInline.wrap===false){this.addCls(baseCls+'-nowrap');}
else{this.removeCls(baseCls+'-nowrap');}}},prepareData:function(data,index,record){return data;},onContainerTrigger:function(e){var me=this;if(e.target!=me.element.dom){return;}
if(me.getDeselectOnContainerClick()&&me.getStore()){me.deselectAll();}},onItemTrigger:function(me,index){this.selectWithEvent(this.getStore().getAt(index));},doAddPressedCls:function(record){var me=this,item=me.container.getViewItems()[me.getStore().indexOf(record)];if(Ext.isElement(item)){item=Ext.get(item);}
if(item){item.addCls(me.getPressedCls());}},onItemTouchStart:function(container,target,index,e){var me=this,store=me.getStore(),record=store&&store.getAt(index);me.fireAction('itemtouchstart',[me,index,target,record,e],'doItemTouchStart');},doItemTouchStart:function(me,index,target,record){var pressedDelay=me.getPressedDelay();if(record){if(pressedDelay>0){me.pressedTimeout=Ext.defer(me.doAddPressedCls,pressedDelay,me,[record]);}
else{me.doAddPressedCls(record);}}},onItemTouchEnd:function(container,target,index,e){var me=this,store=me.getStore(),record=store&&store.getAt(index);if(this.hasOwnProperty('pressedTimeout')){clearTimeout(this.pressedTimeout);delete this.pressedTimeout;}
if(record&&target){target.removeCls(me.getPressedCls());}
me.fireEvent('itemtouchend',me,index,target,record,e);},onItemTouchMove:function(container,target,index,e){var me=this,store=me.getStore(),record=store&&store.getAt(index);if(me.hasOwnProperty('pressedTimeout')){clearTimeout(me.pressedTimeout);delete me.pressedTimeout;}
if(record&&target){target.removeCls(me.getPressedCls());}
me.fireEvent('itemtouchmove',me,index,target,record,e);},onItemTap:function(container,target,index,e){var me=this,store=me.getStore(),record=store&&store.getAt(index);me.fireEvent('itemtap',me,index,target,record,e);},onItemTapHold:function(container,target,index,e){var me=this,store=me.getStore(),record=store&&store.getAt(index);me.fireEvent('itemtaphold',me,index,target,record,e);},onItemSingleTap:function(container,target,index,e){var me=this,store=me.getStore(),record=store&&store.getAt(index);me.fireEvent('itemsingletap',me,index,target,record,e);},onItemDoubleTap:function(container,target,index,e){var me=this,store=me.getStore(),record=store&&store.getAt(index);me.fireEvent('itemdoubletap',me,index,target,record,e);},onItemSwipe:function(container,target,index,e){var me=this,store=me.getStore(),record=store&&store.getAt(index);me.fireEvent('itemswipe',me,index,target,record,e);},onItemSelect:function(record,suppressEvent){var me=this;if(suppressEvent){me.doItemSelect(me,record);}else{me.fireAction('select',[me,record],'doItemSelect');}},doItemSelect:function(me,record){if(me.container){var item=me.container.getViewItems()[me.getStore().indexOf(record)];if(Ext.isElement(item)){item=Ext.get(item);}
if(item){item.removeCls(me.getPressedCls());item.addCls(me.getSelectedCls());}}},onItemDeselect:function(record,suppressEvent){var me=this;if(me.container){if(suppressEvent){me.doItemDeselect(me,record);}
else{me.fireAction('deselect',[me,record,suppressEvent],'doItemDeselect');}}},doItemDeselect:function(me,record){var item=me.container.getViewItems()[me.getStore().indexOf(record)];if(Ext.isElement(item)){item=Ext.get(item);}
if(item){item.removeCls([me.getPressedCls(),me.getSelectedCls()]);}},updateData:function(data){var store=this.getStore();if(!store){this.setStore(Ext.create('Ext.data.Store',{data:data}));}else{store.add(data);}},applyStore:function(store){var me=this,bindEvents=Ext.apply({},me.storeEventHooks,{scope:me}),proxy,reader;if(store){store=Ext.data.StoreManager.lookup(store);if(store&&Ext.isObject(store)&&store.isStore){store.on(bindEvents);proxy=store.getProxy();if(proxy){reader=proxy.getReader();if(reader){reader.on('exception','handleException',this);}}}
else{Ext.Logger.warn("The specified Store cannot be found",this);}}
return store;},handleException:function(){this.setMasked(false);},updateStore:function(newStore,oldStore){var me=this,bindEvents=Ext.apply({},me.storeEventHooks,{scope:me}),proxy,reader;if(oldStore&&Ext.isObject(oldStore)&&oldStore.isStore){if(oldStore.autoDestroy){oldStore.destroy();}
else{oldStore.un(bindEvents);proxy=oldStore.getProxy();if(proxy){reader=proxy.getReader();if(reader){reader.un('exception','handleException',this);}}}}
if(newStore){if(newStore.isLoaded()){this.hasLoadedStore=true;}
if(newStore.isLoading()){me.onBeforeLoad();}
if(me.container){me.refresh();}}},onBeforeLoad:function(){var scrollable=this.getScrollable();if(scrollable){scrollable.getScroller().stopAnimation();}
var loadingText=this.getLoadingText();if(loadingText){this.setMasked({xtype:'loadmask',message:loadingText});if(scrollable){scrollable.getScroller().setDisabled(true);}}
this.hideEmptyText();},updateEmptyText:function(newEmptyText,oldEmptyText){var me=this,store;if(oldEmptyText&&me.emptyTextCmp){me.remove(me.emptyTextCmp,true);delete me.emptyTextCmp;}
if(newEmptyText){me.emptyTextCmp=me.add({xtype:'component',cls:me.getBaseCls()+'-emptytext',html:newEmptyText,hidden:true});store=me.getStore();if(store&&me.hasLoadedStore&&!store.getCount()){this.showEmptyText();}}},onLoad:function(store){var scrollable=this.getScrollable();this.hasLoadedStore=true;this.setMasked(false);if(scrollable){scrollable.getScroller().setDisabled(false);}
if(!store.getCount()){this.showEmptyText();}},refresh:function(){var me=this,container=me.container;if(!me.getStore()){if(!me.hasLoadedStore&&!me.getDeferEmptyText()){me.showEmptyText();}
return;}
if(container){me.fireAction('refresh',[me],'doRefresh');}},applyItemTpl:function(config){return(Ext.isObject(config)&&config.isTemplate)?config:new Ext.XTemplate(config);},onAfterRender:function(){var me=this;me.callParent(arguments);me.updateStore(me.getStore());},getViewItems:function(){return this.container.getViewItems();},doRefresh:function(me){var container=me.container,store=me.getStore(),records=store.getRange(),items=container.getViewItems(),recordsLn=records.length,itemsLn=items.length,deltaLn=recordsLn-itemsLn,scrollable=me.getScrollable(),i,item;if(this.getScrollToTopOnRefresh()&&scrollable){scrollable.getScroller().scrollToTop();}
if(recordsLn<1){me.onStoreClear();return;}
if(deltaLn<0){container.moveItemsToCache(itemsLn+deltaLn,itemsLn-1);items=container.getViewItems();itemsLn=items.length;}
else if(deltaLn>0){container.moveItemsFromCache(store.getRange(itemsLn));}
for(i=0;i<itemsLn;i++){item=items[i];container.updateListItem(records[i],item);}},showEmptyText:function(){if(this.getEmptyText()&&(this.hasLoadedStore||!this.getDeferEmptyText())){this.emptyTextCmp.show();}},hideEmptyText:function(){if(this.getEmptyText()){this.emptyTextCmp.hide();}},onStoreClear:function(){var me=this,container=me.container,items=container.getViewItems();container.moveItemsToCache(0,items.length-1);this.showEmptyText();},onStoreAdd:function(store,records){if(records){this.container.moveItemsFromCache(records);}},onStoreRemove:function(store,records,indices){var container=this.container,ln=records.length,i;for(i=0;i<ln;i++){container.moveItemsToCache(indices[i],indices[i]);}},onStoreUpdate:function(store,record,newIndex,oldIndex){var me=this,container=me.container;oldIndex=(typeof oldIndex==='undefined')?newIndex:oldIndex;if(oldIndex!==newIndex){container.moveItemsToCache(oldIndex,oldIndex);container.moveItemsFromCache([record]);}
else{container.updateListItem(record,container.getViewItems()[newIndex]);}}},function(){Ext.deprecateMethod(this,'collectData',null,"Ext.dataview.DataView.collectData() has been removed");Ext.deprecateMethod(this,'findItemByChild',null,"Ext.dataview.DataView.findItemByChild() has been removed");Ext.deprecateMethod(this,'findTargetByEvent',null,"Ext.dataview.DataView.findTargetByEvent() has been removed");Ext.deprecateMethod(this,'getNode',null,"Ext.dataview.DataView.getNode() has been removed");Ext.deprecateMethod(this,'getNodes',null,"Ext.dataview.DataView.getNodes() has been removed");Ext.deprecateMethod(this,'getRecords',null,"Ext.dataview.DataView.getRecords() has been removed");Ext.deprecateMethod(this,'getSelectedNodes',null,"Ext.dataview.DataView.getSelectedNodes() has been removed");Ext.deprecateMethod(this,'getSelectedRecords',null,"Ext.dataview.DataView.getSelectedRecords() has been removed");Ext.deprecateMethod(this,'indexOf',null,"Ext.dataview.DataView.indexOf() has been removed");Ext.deprecateMethod(this,'refreshNode',null,"Ext.dataview.DataView.refreshNode() has been removed");Ext.deprecateClassMethod(this,'bindStore','setStore');Ext.deprecateProperty(this,'blockRefresh',null,"Ext.dataview.DataView.blockRefresh has been removed");Ext.deprecateProperty(this,'itemSelector',null,"Ext.dataview.DataView.itemSelector has been removed");Ext.deprecateProperty(this,'multiSelect',null,"Ext.dataview.DataView.multiSelect has been removed");Ext.deprecateProperty(this,'overItemCls',null,"Ext.dataview.DataView.overItemCls has been removed");Ext.deprecateProperty(this,'selectedItemCls',null,"Ext.dataview.DataView.selectedItemCls has been removed");Ext.deprecateProperty(this,'simpleSelect',null,"Ext.dataview.DataView.simpleSelect has been removed");Ext.deprecateProperty(this,'singleSelect',null,"Ext.dataview.DataView.singleSelect has been removed");Ext.deprecateProperty(this,'trackOver',null,"Ext.dataview.DataView.trackOver has been removed");});Ext.define('Ext.dataview.List',{alternateClassName:'Ext.List',extend:'Ext.dataview.DataView',xtype:'list',requires:['Ext.dataview.element.List','Ext.dataview.IndexBar','Ext.dataview.ListItemHeader'],config:{indexBar:false,icon:null,preventSelectionOnDisclose:true,baseCls:Ext.baseCSSPrefix+'list',pinHeaders:true,grouped:false,onItemDisclosure:null,disclosureProperty:'disclosure',ui:'normal'},constructor:function(){this.translateHeader=(Ext.os.is.Android2)?this.translateHeaderCssPosition:this.translateHeaderTransform;this.callParent(arguments);},onItemTrigger:function(me,index,target,record,e){if(!(this.getPreventSelectionOnDisclose()&&Ext.fly(e.target).hasCls(this.getBaseCls()+'-disclosure'))){this.callParent(arguments);}},initialize:function(){var me=this,container;me.on(me.getTriggerCtEvent(),me.onContainerTrigger,me);container=me.container=this.add(new Ext.dataview.element.List({baseCls:this.getBaseCls()}));container.dataview=me;me.on(me.getTriggerEvent(),me.onItemTrigger,me);container.element.on({delegate:'.'+this.getBaseCls()+'-disclosure',tap:'handleItemDisclosure',scope:me});container.on({itemtouchstart:'onItemTouchStart',itemtouchend:'onItemTouchEnd',itemtap:'onItemTap',itemtaphold:'onItemTapHold',itemtouchmove:'onItemTouchMove',itemsingletap:'onItemSingleTap',itemdoubletap:'onItemDoubleTap',itemswipe:'onItemSwipe',scope:me});if(this.getStore()){this.refresh();}},updateInline:function(newInline){this.callParent(arguments);if(newInline){this.setOnItemDisclosure(false);this.setIndexBar(false);this.setGrouped(false);}},applyIndexBar:function(indexBar){return Ext.factory(indexBar,Ext.dataview.IndexBar,this.getIndexBar());},updateIndexBar:function(indexBar){if(indexBar&&this.getScrollable()){this.indexBarElement=this.getScrollableBehavior().getScrollView().getElement().appendChild(indexBar.renderElement);indexBar.on({index:'onIndex',scope:this});this.element.addCls(this.getBaseCls()+'-indexed');}},updateGrouped:function(grouped){var baseCls=this.getBaseCls(),cls=baseCls+'-grouped',unCls=baseCls+'-ungrouped';if(grouped){this.addCls(cls);this.removeCls(unCls);this.doRefreshHeaders();this.updatePinHeaders(this.getPinHeaders());}
else{this.addCls(unCls);this.removeCls(cls);if(this.container){this.container.doRemoveHeaders();}
this.updatePinHeaders(null);}},updatePinHeaders:function(pinnedHeaders){var scrollable=this.getScrollable(),scroller;if(scrollable){scroller=scrollable.getScroller();}
if(!scrollable){return;}
if(pinnedHeaders&&this.getGrouped()){scroller.on({refresh:'doRefreshHeaders',scroll:'onScroll',scope:this});if(!this.header||!this.header.renderElement.dom){this.createHeader();}}else{scroller.un({refresh:'doRefreshHeaders',scroll:'onScroll',scope:this});if(this.header){this.header.destroy();}}},createHeader:function(){var header,scrollable=this.getScrollable(),scroller,scrollView,scrollViewElement;if(scrollable){scroller=scrollable.getScroller();scrollView=this.getScrollableBehavior().getScrollView();scrollViewElement=scrollView.getElement();}
else{return;}
this.header=header=Ext.create('Ext.dataview.ListItemHeader',{html:' ',cls:'x-list-header-swap'});scrollViewElement.dom.insertBefore(header.element.dom,scroller.getContainer().dom.nextSibling);this.translateHeader(1000);},refresh:function(){this.callParent();this.doRefreshHeaders();},onStoreAdd:function(){this.callParent(arguments);this.doRefreshHeaders();},onStoreRemove:function(){this.callParent(arguments);this.doRefreshHeaders();},onStoreUpdate:function(){this.callParent(arguments);this.doRefreshHeaders();},onStoreClear:function(){this.callParent();if(this.header){this.header.destroy();}
this.doRefreshHeaders();},getClosestGroups:function(){var groups=this.pinHeaderInfo.offsets,scrollable=this.getScrollable(),ln=groups.length,i=0,pos,group,current,next;if(scrollable){pos=scrollable.getScroller().position;}
else{return{current:0,next:0};}
for(;i<ln;i++){group=groups[i];if(group.offset>pos.y){next=group;break;}
current=group;}
return{current:current,next:next};},doRefreshHeaders:function(){if(!this.getGrouped()||!this.container){return false;}
var headerIndices=this.findGroupHeaderIndices(),ln=headerIndices.length,items=this.container.getViewItems(),headerInfo=this.pinHeaderInfo={offsets:[]},headerOffsets=headerInfo.offsets,scrollable=this.getScrollable(),scroller,scrollPosition,i,headerItem,header;if(ln){for(i=0;i<ln;i++){headerItem=items[headerIndices[i]];if(headerItem){header=this.getItemHeader(headerItem);headerOffsets.push({header:header,offset:headerItem.offsetTop});}}
headerInfo.closest=this.getClosestGroups();this.setActiveGroup(headerInfo.closest.current);if(header){headerInfo.headerHeight=Ext.fly(header).getHeight();}
if(scrollable){scroller=scrollable.getScroller();scrollPosition=scroller.position;this.onScroll(scroller,scrollPosition.x,scrollPosition.y);}}},getItemHeader:function(item){var element=Ext.fly(item).down(this.container.headerClsCache);return element?element.dom:null;},onScroll:function(scroller,x,y){var me=this,headerInfo=me.pinHeaderInfo,closest=headerInfo.closest,activeGroup=me.activeGroup,headerHeight=headerInfo.headerHeight,next,current;if(!closest){return;}
next=closest.next;current=closest.current;if(!this.header||!this.header.renderElement.dom){this.createHeader();}
if(y<=0){if(activeGroup){me.setActiveGroup(false);closest.next=current;}
this.translateHeader(1000);return;}
else if((next&&y>next.offset)||(current&&y<current.offset)){closest=headerInfo.closest=this.getClosestGroups();next=closest.next;current=closest.current;this.setActiveGroup(current);}
if(next&&y>0&&next.offset-y<=headerHeight){var headerOffset=headerHeight-(next.offset-y);this.translateHeader(headerOffset);}
else{this.translateHeader(null);}},translateHeaderTransform:function(offset){this.header.renderElement.dom.style.webkitTransform=(offset===null)?null:'translate3d(0px, -'+offset+'px, 0px)';},translateHeaderCssPosition:function(offset){this.header.renderElement.dom.style.top=(offset===null)?null:'-'+Math.round(offset)+'px';},setActiveGroup:function(group){var me=this,header=me.header;if(header){if(group&&group.header){if(!me.activeGroup||me.activeGroup.header!=group.header){header.show();if(header.element){header.setHtml(group.header.innerHTML);}}}else if(header&&header.element){header.hide();}}
this.activeGroup=group;},onIndex:function(indexBar,index){var me=this,key=index.toLowerCase(),store=me.getStore(),groups=store.getGroups(),ln=groups.length,scrollable=me.getScrollable(),scroller,group,i,closest,id,item;if(scrollable){scroller=me.getScrollable().getScroller();}
else{return;}
for(i=0;i<ln;i++){group=groups[i];id=group.name.toLowerCase();if(id==key||id>key){closest=group;break;}
else{closest=group;}}
if(scrollable&&closest){item=me.container.getViewItems()[store.indexOf(closest.children[0])];scroller.stopAnimation();var containerSize=scroller.getContainerSize().y,size=scroller.getSize().y,maxOffset=size-containerSize,offset=(item.offsetTop>maxOffset)?maxOffset:item.offsetTop;scroller.scrollTo(0,offset);}},applyOnItemDisclosure:function(config){if(Ext.isFunction(config)){return{scope:this,handler:config};}
return config;},handleItemDisclosure:function(e){var me=this,item=e.getTarget().parentNode,index=me.container.getViewItems().indexOf(item),record=me.getStore().getAt(index);me.fireAction('disclose',[me,record,item,index,e],'doDisclose');},doDisclose:function(me,record,item,index,e){var onItemDisclosure=me.getOnItemDisclosure();if(onItemDisclosure&&onItemDisclosure.handler){onItemDisclosure.handler.call(me,record,item,index,e);}},findGroupHeaderIndices:function(){if(!this.getGrouped()){return[];}
var me=this,store=me.getStore();if(!store){return[];}
var container=me.container,groups=store.getGroups(),groupLn=groups.length,items=container.getViewItems(),newHeaderItems=[],footerClsShortCache=container.footerClsShortCache,i,firstGroupedRecord,index,item,lastGroup;container.doRemoveHeaders();container.doRemoveFooterCls();if(items.length){for(i=0;i<groupLn;i++){firstGroupedRecord=groups[i].children[0];index=store.indexOf(firstGroupedRecord);item=items[index];container.doAddHeader(item,store.getGroupString(firstGroupedRecord));if(i){Ext.fly(item.previousSibling).addCls(footerClsShortCache);}
newHeaderItems.push(index);}
lastGroup=groups[--i].children;Ext.fly(items[store.indexOf(lastGroup[lastGroup.length-1])]).addCls(footerClsShortCache);}
return newHeaderItems;},destroy:function(){Ext.destroy(this.getIndexBar(),this.indexBarElement,this.header);this.callParent();}});Ext.define('zvsMobile.view.DeviceList',{extend:'Ext.dataview.List',requires:['Ext.plugin.PullRefresh'],xtype:'DeviceList',config:{cls:'DeviceListItem',store:DeviceStore,scrollable:'vertical',ui:'round',scrollToTopOnRefresh:false,plugins:[{xclass:'Ext.plugin.PullRefresh'}],itemTpl:new Ext.XTemplate('<div class="device">','<div class="imageholder {type}_{on_off}"></div>','<h2>{name}</h2>','<div class="level">','<div class="meter">','<div class="progress" style="width:{level}%">','</div>','</div>','<div class="percent">{level_txt}</div>','</div>','</div>'),items:[{xtype:'toolbar',docked:'top',title:'Devices'}]}});Ext.define('zvsMobile.view.phone.DevicePhoneViewPort',{extend:'Ext.Panel',xtype:'DevicePhoneViewPort',requires:['zvsMobile.view.DeviceList','zvsMobile.view.DeviceDetailsDimmer','zvsMobile.view.DeviceDetailsSwitch','zvsMobile.view.DeviceDetailsThermo'],initialize:function(){this.callParent(arguments);},config:{layout:{type:'card',animation:{type:'slide',direction:'left'}},items:[{xtype:'DeviceList',id:'DeviceList',listeners:{scope:this,selectionchange:function(list,records){if(records[0]!==undefined){var DevicePhoneViewPort=Ext.getCmp('DevicePhoneViewPort');var DimmmerDetails=Ext.getCmp('DeviceDetailsDimmer');var SwitchDetails=Ext.getCmp('DeviceDetailsSwitch');var TempDetails=Ext.getCmp('DeviceDetailsThermo');if(records[0].data.type==='DIMMER'){DimmmerDetails.loadDevice(records[0].data.id);DevicePhoneViewPort.getLayout().setAnimation({type:'slide',direction:'left'});DevicePhoneViewPort.setActiveItem(Ext.getCmp('DimmerView'));}
if(records[0].data.type==='SWITCH'){SwitchDetails.loadDevice(records[0].data.id);DevicePhoneViewPort.getLayout().setAnimation({type:'slide',direction:'left'});DevicePhoneViewPort.setActiveItem(Ext.getCmp('SwitchView'));}
if(records[0].data.type==='THERMOSTAT'){TempDetails.loadDevice(records[0].data.id);DevicePhoneViewPort.getLayout().setAnimation({type:'slide',direction:'left'});DevicePhoneViewPort.setActiveItem(Ext.getCmp('ThermoView'));}}},activate:function(){Ext.getCmp('DeviceList').deselectAll();}}},{layout:'card',id:'DimmerView',items:[{xtype:'DeviceDetailsDimmer',id:'DeviceDetailsDimmer'},{xtype:'toolbar',docked:'top',title:'Device Details',scrollable:false,items:[{xtype:'button',iconMask:true,ui:'back',text:'Back',handler:function(){var DevicePhoneViewPort=Ext.getCmp('DevicePhoneViewPort');DevicePhoneViewPort.getLayout().setAnimation({type:'slide',direction:'right'});DevicePhoneViewPort.setActiveItem(Ext.getCmp('DeviceList'));}}]}]},{layout:'card',id:'SwitchView',items:[{xtype:'DeviceDetailsSwitch',id:'DeviceDetailsSwitch'},{xtype:'toolbar',docked:'top',title:'Device Details',scrollable:false,items:[{xtype:'button',iconMask:true,ui:'back',text:'Back',handler:function(){var DevicePhoneViewPort=Ext.getCmp('DevicePhoneViewPort');DevicePhoneViewPort.getLayout().setAnimation({type:'slide',direction:'right'});DevicePhoneViewPort.setActiveItem(Ext.getCmp('DeviceList'));}}]}]},{layout:'card',id:'ThermoView',items:[{xtype:'DeviceDetailsThermo',id:'DeviceDetailsThermo'},{xtype:'toolbar',docked:'top',title:'Device Details',scrollable:false,items:[{xtype:'button',iconMask:true,ui:'back',text:'Back',handler:function(){var DevicePhoneViewPort=Ext.getCmp('DevicePhoneViewPort');DevicePhoneViewPort.getLayout().setAnimation({type:'slide',direction:'right'});DevicePhoneViewPort.setActiveItem(Ext.getCmp('DeviceList'));}}]}]}]}});Ext.define('zvsMobile.view.tablet.DeviceTabletViewPort',{extend:'Ext.Panel',xtype:'DeviceTabletViewPort',requires:['zvsMobile.view.DeviceList','zvsMobile.view.DeviceDetailsDimmer','zvsMobile.view.DeviceDetailsSwitch','zvsMobile.view.DeviceDetailsThermo'],initialize:function(){this.callParent(arguments);},config:{layout:'hbox',items:[{id:'DeviceList',xtype:'DeviceList',listeners:{scope:this,selectionchange:function(list,records){if(records[0]!==undefined){var deviceDetailsPane=Ext.getCmp('deviceDetailsPane');if(records[0].data.type==='DIMMER'){var DimmmerDetails=deviceDetailsPane.items.items[1];DimmmerDetails.loadDevice(records[0].data.id);deviceDetailsPane.getLayout().setAnimation({type:'slide',direction:'up'});deviceDetailsPane.setActiveItem(DimmmerDetails);DimmmerDetails.ShowBackButton();}
if(records[0].data.type==='SWITCH'){var SWITCHDetails=deviceDetailsPane.items.items[2];SWITCHDetails.loadDevice(records[0].data.id);deviceDetailsPane.getLayout().setAnimation({type:'slide',direction:'up'});deviceDetailsPane.setActiveItem(SWITCHDetails);}
if(records[0].data.type==='THERMOSTAT'){var ThermoDetails=deviceDetailsPane.items.items[3];ThermoDetails.loadDevice(records[0].data.id);deviceDetailsPane.getLayout().setAnimation({type:'slide',direction:'up'});deviceDetailsPane.setActiveItem(ThermoDetails);}}}},flex:1},{flex:2,id:'deviceDetailsPane',layout:{type:'card',animation:{type:'slide',direction:'left'}},items:[{cls:'emptyDetail',html:"Select a device to see more details."},{xtype:'DeviceDetailsDimmer',id:'DeviceDetailsDimmer'},{xtype:'DeviceDetailsSwitch',id:'DeviceDetailsSwitch'},{xtype:'DeviceDetailsThermo',id:'DeviceDetailsThermo'},{xtype:'toolbar',docked:'top',title:'Device Details',scrollable:false}]}]}});Ext.define('zvsMobile.view.SceneList',{extend:'Ext.dataview.List',requires:['Ext.plugin.PullRefresh'],xtype:'SceneList',config:{cls:'SceneListItem',store:SceneStore,scrollable:'vertical',ui:'round',scrollToTopOnRefresh:false,plugins:[{xclass:'Ext.plugin.PullRefresh'}],itemTpl:new Ext.XTemplate('<div class="scene">','<div class="imageholder running_{is_running}"></div>','<h2>{name} ({cmd_count})</h2>','</div>'),items:{xtype:'toolbar',docked:'top',title:'Scenes'}}});Ext.define('zvsMobile.view.phone.ScenePhoneViewPort',{extend:'Ext.Panel',xtype:'ScenePhoneViewPort',requires:['zvsMobile.view.SceneList','zvsMobile.view.SceneDetails'],initialize:function(){this.callParent(arguments);},config:{layout:{type:'card',animation:{type:'slide',direction:'left'}},items:[{xtype:'SceneList',id:'SceneList',listeners:{scope:this,selectionchange:function(list,records){if(records[0]!==undefined){var SceneDetails=Ext.getCmp('SceneDetails');SceneDetails.loadScene(records[0].data.id);var ScenePhoneViewPort=Ext.getCmp('ScenePhoneViewPort');ScenePhoneViewPort.getLayout().setAnimation({type:'slide',direction:'left'});ScenePhoneViewPort.setActiveItem(Ext.getCmp('SceneDetailContainer'));}},activate:function(){Ext.getCmp('DeviceList').deselectAll();}}},{id:'SceneDetailContainer',layout:'card',items:[{xtype:'SceneDetails',id:'SceneDetails'},{xtype:'toolbar',docked:'top',title:'Scene Details',scrollable:false,items:[{xtype:'button',iconMask:true,ui:'back',text:'Back',handler:function(){var ScenePhoneViewPort=Ext.getCmp('ScenePhoneViewPort');ScenePhoneViewPort.getLayout().setAnimation({type:'slide',direction:'right'});ScenePhoneViewPort.setActiveItem(Ext.getCmp('SceneList'));}}]}]}]}});Ext.define('zvsMobile.view.tablet.SceneTabletViewPort',{extend:'Ext.Panel',xtype:'SceneTabletViewPort',requires:['zvsMobile.view.SceneList','zvsMobile.view.SceneDetails'],config:{layout:'hbox',items:[{xtype:'SceneList',id:'SceneList',flex:1,listeners:{scope:this,selectionchange:function(list,records){if(records[0]!==undefined){var SceneDetails=Ext.getCmp('SceneDetails');SceneDetails.loadScene(records[0].data.id);var sceneDetailsPane=Ext.getCmp('sceneDetailsPane');sceneDetailsPane.getLayout().setAnimation({type:'slide',direction:'up'});sceneDetailsPane.setActiveItem(SceneDetails);}}}},{flex:2,id:'sceneDetailsPane',layout:{type:'card',animation:{type:'slide',direction:'left'}},items:[{cls:'emptyDetail',html:"Select a scene to see more details."},{xtype:'SceneDetails',id:'SceneDetails'},{xtype:'toolbar',docked:'top',title:'Scene Details'}]}]}});Ext.define('zvsMobile.view.GroupList',{extend:'Ext.dataview.List',requires:['Ext.plugin.PullRefresh'],xtype:'GroupList',config:{cls:'GroupList',store:GroupStore,scrollable:'vertical',ui:'round',scrollToTopOnRefresh:false,plugins:[{xclass:'Ext.plugin.PullRefresh'}],itemTpl:new Ext.XTemplate('<div class="group">','<div class="imageholder"></div>','<h1>{name} ({count})</h1>','</div>'),items:{xtype:'toolbar',docked:'top',title:'Groups'}}});Ext.define('zvsMobile.view.phone.GroupPhoneViewPort',{extend:'Ext.Panel',xtype:'GroupPhoneViewPort',requires:['zvsMobile.view.DeviceList','zvsMobile.view.GroupList','zvsMobile.view.GroupDetails'],config:{layout:{type:'card',animation:{type:'slide',direction:'left'}},items:[{xtype:'GroupList',id:'GroupList',listeners:{scope:this,selectionchange:function(list,records){if(records[0]!==undefined){var GroupDetails=Ext.getCmp('GroupDetails');GroupDetails.loadGroup(records[0].data.id);var GroupPhoneViewPort=Ext.getCmp('GroupPhoneViewPort');GroupPhoneViewPort.getLayout().setAnimation({type:'slide',direction:'left'});GroupPhoneViewPort.setActiveItem(Ext.getCmp('GroupDetailContainer'));}},activate:function(){Ext.getCmp('GroupList').deselectAll();}}},{id:'GroupDetailContainer',layout:'card',items:[{xtype:'GroupDetails',id:'GroupDetails'},{xtype:'toolbar',docked:'top',title:'Group Details',scrollable:false,items:[{xtype:'button',iconMask:true,ui:'back',text:'Back',handler:function(){var GroupPhoneViewPort=Ext.getCmp('GroupPhoneViewPort');GroupPhoneViewPort.getLayout().setAnimation({type:'slide',direction:'right'});GroupPhoneViewPort.setActiveItem(Ext.getCmp('GroupList'));}}]}]}]}});Ext.define('zvsMobile.view.tablet.GroupTabletViewPort',{extend:'Ext.Panel',xtype:'GroupTabletViewPort',requires:['zvsMobile.view.DeviceList','zvsMobile.view.GroupList','zvsMobile.view.GroupDetails'],config:{layout:'hbox',items:[{xtype:'GroupList',id:'GroupList',flex:1,listeners:{scope:this,selectionchange:function(list,records){if(records[0]!==undefined){var GroupDetails=Ext.getCmp('GroupDetails');GroupDetails.loadGroup(records[0].data.id);var groupDetailsPane=Ext.getCmp('groupDetailsPane');groupDetailsPane.getLayout().setAnimation({type:'slide',direction:'up'});groupDetailsPane.setActiveItem(GroupDetails);}}}},{flex:2,id:'groupDetailsPane',layout:{type:'card',animation:{type:'slide',direction:'left'}},items:[{cls:'emptyDetail',html:"Select a group to see more details."},{xtype:'GroupDetails',id:'GroupDetails'},{xtype:'toolbar',docked:'top',title:'Group Details'}]}]}});Ext.define('Ext.data.ArrayStore',{extend:'Ext.data.Store',alias:'store.array',uses:['Ext.data.reader.Array'],config:{proxy:{type:'memory',reader:'array'}},loadData:function(data,append){this.callParent([data,append]);}},function(){Ext.data.SimpleStore=Ext.data.ArrayStore;});Ext.define('Ext.data.reader.Array',{extend:'Ext.data.reader.Json',alternateClassName:'Ext.data.ArrayReader',alias:'reader.array',config:{totalProperty:undefined,successProperty:undefined},createFieldAccessExpression:function(field,fieldVarName,dataName){var me=this,mapping=field.getMapping(),index=(mapping==null)?me.getModel().getFields().indexOf(field):mapping,result;if(typeof index==='function'){result=fieldVarName+'.getMapping()('+dataName+', this)';}else{if(isNaN(index)){index='"'+index+'"';}
result=dataName+"["+index+"]";}
return result;}});
/**
 * @private
 *
 * A general {@link Ext.picker.Picker} slot class.  Slots are used to organize multiple scrollable slots into
 * a single {@link Ext.picker.Picker}.
 *
 *     {
 *         name : 'limit_speed',
 *         title: 'Speed Limit',
 *         data : [
 *             {text: '50 KB/s', value: 50},
 *             {text: '100 KB/s', value: 100},
 *             {text: '200 KB/s', value: 200},
 *             {text: '300 KB/s', value: 300}
 *         ]
 *     }
 *
 * See the {@link Ext.picker.Picker} documentation on how to use slots.
 */
Ext.define('Ext.picker.Slot', {
    extend: 'Ext.DataView',
    xtype : 'pickerslot',
    alternateClassName: 'Ext.Picker.Slot',
    requires: [
        'Ext.XTemplate',
        'Ext.data.Store',
        'Ext.Component',
        'Ext.data.StoreManager'
    ],

    /**
     * @event slotpick
     * Fires whenever an slot is picked
     * @param {Ext.picker.Slot} this
     * @param {Mixed} value The value of the pick
     * @param {HTMLElement} node The node element of the pick
     */

    isSlot: true,

    config: {
        /**
         * @cfg {String} title
         * The title to use for this slot. Null for no title
         * @accessor
         */
        title: null,

        /**
         * @private
         * @cfg {Boolean} showTitle
         * @accessor
         */
        showTitle: true,

        /**
         * @private
         * @cfg {String} cls
         * The main component class
         * @accessor
         */
        cls: Ext.baseCSSPrefix + 'picker-slot',

        /**
         * @cfg {String} name
         * The name of this slot. This config option is required.
         * @accessor
         */
        name: null,

        /**
         * @cfg {Number} value The value of this slot
         * @accessor
         */
        value: null,

        /**
         * @cfg {Number} flex
         * @accessor
         * @hide
         */
        flex: 1,

        /**
         * @cfg {String} align
         * The horizontal alignment of the slot's contents. Valid values are "left", "center",
         * and "right". Defaults to "left".
         * @accessor
         */
        align: 'left',

        /**
         * @cfg {String} displayField
         * The display field in the store.
         * Defaults to 'text'.
         * @accessor
         */
        displayField: 'text',

        /**
         * @cfg {String} valueField
         * The value field in the store.
         * Defaults to 'value'.
         * @accessor
         */
        valueField: 'value',

        /**
         * @cfg {Object} scrollable
         * @accessor
         * @hide
         */
        scrollable: {
            direction: 'vertical',
            indicators: false,
            momentumEasing: {
                minVelocity: 2
            },
            slotSnapEasing: {
                duration: 100
            }
        }
    },

    constructor: function() {
        /**
         * @property selectedIndex
         * @type Number
         * The current selectedIndex of the picker slot
         * @private
         */
        this.selectedIndex = 0;

        /**
         * @property picker
         * @type Ext.picker.Picker
         * A reference to the owner Picker
         * @private
         */

        this.callParent(arguments);
    },

    /**
     * Sets the title for this dataview by creating element
     */
    applyTitle: function(title) {
        //check if the title isnt defined
        if (title) {
            //create a new title element
            title = Ext.create('Ext.Component', {
                cls: Ext.baseCSSPrefix + 'picker-slot-title',
                docked      : 'top',
                html        : title
            });
        }

        return title;
    },

    updateTitle: function(newTitle, oldTitle) {
        if (newTitle) {
            this.add(newTitle);
            this.setupBar();
        }

        if (oldTitle) {
            this.remove(oldTitle);
        }
    },

    updateShowTitle: function(showTitle) {
        var title = this.getTitle();
        if (title) {
            title[showTitle ? 'show' : 'hide']();

            this.setupBar();
        }
    },

    updateDisplayField: function(newDisplayField) {
        this.setItemTpl('<div class="' + Ext.baseCSSPrefix + 'picker-item {cls} <tpl if="extra">' + Ext.baseCSSPrefix + 'picker-invalid</tpl>">{' + newDisplayField + '}</div>');
    },

    /**
     * Updates the {@link #align} configuration
     */
    updateAlign: function(newAlign, oldAlign) {
        var element = this.element;
        element.addCls(Ext.baseCSSPrefix + 'picker-' + newAlign);
        element.removeCls(Ext.baseCSSPrefix + 'picker-' + oldAlign);
    },

    /**
     * Looks at the {@link #data} configuration and turns it into {@link #store}
     */
    applyData: function(data) {
        var parsedData = [],
            ln = data && data.length,
            i, item, obj;

        if (data && Ext.isArray(data) && ln) {
            for (i = 0; i < ln; i++) {
                item = data[i];
                obj = {};
                if (Ext.isArray(item)) {
                    obj[this.valueField] = item[0];
                    obj[this.displayField] = item[1];
                }
                else if (Ext.isString(item)) {
                    obj[this.valueField] = item;
                    obj[this.displayField] = item;
                }
                else if (Ext.isObject(item)) {
                    obj = item;
                }
                parsedData.push(obj);
            }
        }

        return data;
    },

    updateData: function(data) {
        this.setStore(Ext.create('Ext.data.Store', {
            model: 'x-textvalue',
            data : data
        }));
    },

    // @private
    initialize: function() {
        this.callParent();

        var scroller = this.getScrollable().getScroller();

        this.on({
            scope: this,
            painted: 'onPainted',
            itemtap: 'doItemTap'
        });

        scroller.on({
            scope: this,
            scrollend: 'onScrollEnd'
        });
    },

    // @private
    onPainted: function() {
        this.setupBar();
    },

    /**
     * Returns an instance of the owner picker
     * @private
     */
    getPicker: function() {
        if (!this.picker) {
            this.picker = this.getParent();
        }

        return this.picker;
    },

    // @private
    setupBar: function() {
        if (!this.rendered) {
            //if the component isnt rendered yet, there is no point in calculating the padding just eyt
            return;
        }

        var element = this.element,
            innerElement = this.innerElement,
            picker = this.getPicker(),
            bar = picker.bar,
            value = this.getValue(),
            showTitle = this.getShowTitle(),
            title = this.getTitle(),
            scrollable = this.getScrollable(),
            scroller = scrollable.getScroller(),
            titleHeight = 0,
            barY, elY, barHeight, padding, paddingBottom;

        barY = bar.getY();
        elY = element.getY();

        if (showTitle && title) {
            elY += title.element.getHeight();
        }

        barHeight = bar.getHeight();

        if (showTitle && title) {
            titleHeight = title.element.getHeight();
        }

        padding = Math.ceil((element.getHeight() - titleHeight - barHeight) / 2);
        this.slotPadding = padding;

        innerElement.setStyle({
            padding: padding + 'px 0 ' + (padding) + 'px'
        });

        scroller.refresh();
        scroller.setSlotSnapSize(barHeight);

        this.setValue(value);
    },

    // @private
    doItemTap: function(list, index, item, e) {
        var me = this;
        me.selectedIndex = index;
        me.selectedNode = item;
        me.scrollToItem(item, true);

        me.fireEvent('slotpick', me, me.getValue(), me.selectedNode);
    },

    // @private
    scrollToItem: function(item, animated) {
        var y = item.getY(),
            parentEl = item.parent(),
            parentY = parentEl.getY(),
            // padding = this.slotPadding,
            scrollView = this.getScrollable(),
            scroller = scrollView.getScroller(),
            difference;

        difference = y - parentY;

        scroller.scrollTo(0, difference, animated);
    },

    // @private
    onScrollEnd: function(scroller, x, y) {
        var me = this,
            index = Math.round(y / me.picker.bar.getHeight()),
            viewItems = me.getViewItems(),
            item = viewItems[index];

        if (item) {
            me.selectedIndex = index;
            me.selectedNode = item;

            me.fireEvent('slotpick', me, me.getValue(), me.selectedNode);
        }
    },

    /**
     * Returns the vlaue of this slot
     * @private
     */
    getValue: function() {
        var store = this.getStore(),
            record, value;

        if (!store) {
            return;
        }

        if (!this.rendered) {
            return this._value;
        }

        //if the value is ever false, that means we do not want to return anything
        if (this._value === false) {
            return null;
        }

        record = store.getAt(this.selectedIndex);

        value = record ? record.get(this.getValueField()) : null;
        this._value = value;

        return value;
    },

    /**
     * Sets the value of this slot
     * @private
     */
    setValue: function(value) {
        if (!Ext.isDefined(value)) {
            return;
        }

        if (!this.rendered || !value) {
            //we don't want to call this until the slot has been rendered
            this._value = value;
            return;
        }

        var store = this.getStore(),
            viewItems = this.getViewItems(),
            valueField = this.getValueField(),
            index, item;

        index = store.find(valueField, value);
        if (index != -1) {
            item = Ext.get(viewItems[index]);

            this.selectedIndex = index;
            this.scrollToItem(item);

            this._value = value;
        }
    },

    /**
     * Sets the value of this slot
     * @private
     */
    setValueAnimated: function(value) {
        if (!value) {
            return;
        }

        if (!this.rendered) {
            //we don't want to call this until the slot has been rendered
            this._value = value;
            return;
        }

        var store = this.getStore(),
            viewItems = this.getViewItems(),
            valueField = this.getValueField(),
            index, item;

        index = store.find(valueField, value);
        if (index != -1) {
            item = Ext.get(viewItems[index]);

            this.selectedIndex = index;
            this.scrollToItem(item, {
                duration: 100
            });

            this._value = value;
        }
    }
});
/**
A general picker class. {@link Ext.picker.Slot}s are used to organize multiple scrollable slots into a single picker. {@link #slots} is
the only necessary configuration.

The {@link #slots} configuration with a few key values:

 - **name:** The name of the slot (will be the key when using {@link #getValues} in this {@link Ext.picker.Picker})
 - **title:** The title of this slot (if {@link #useTitles} is set to true)
 - **data/store:** The data or store to use for this slot.

Remember, {@link Ext.picker.Slot} class extends from {@link Ext.dataview.DataView}.

## Examples

    @example miniphone preview
    var picker = Ext.create('Ext.Picker', {
        slots: [
            {
                name : 'limit_speed',
                title: 'Speed',
                data : [
                    {text: '50 KB/s', value: 50},
                    {text: '100 KB/s', value: 100},
                    {text: '200 KB/s', value: 200},
                    {text: '300 KB/s', value: 300}
                ]
            }
        ]
    });
    Ext.Viewport.add(picker);
    picker.show();

You can also customize the top toolbar on the {@link Ext.picker.Picker} by changing the {@link #doneButton} and {@link #cancelButton} configurations:

    @example miniphone preview
    var picker = Ext.create('Ext.Picker', {
        doneButton: 'I\'m done!',
        cancelButton: false,
        slots: [
            {
                name : 'limit_speed',
                title: 'Speed',
                data : [
                    {text: '50 KB/s', value: 50},
                    {text: '100 KB/s', value: 100},
                    {text: '200 KB/s', value: 200},
                    {text: '300 KB/s', value: 300}
                ]
            }
        ]
    });
    Ext.Viewport.add(picker);
    picker.show();

Or by passing a custom {@link #toolbar} configuration:

    @example miniphone preview
    var picker = Ext.create('Ext.Picker', {
        doneButton: false,
        cancelButton: false,
        toolbar: {
            ui: 'light',
            title: 'My Picker!'
        },
        slots: [
            {
                name : 'limit_speed',
                title: 'Speed',
                data : [
                    {text: '50 KB/s', value: 50},
                    {text: '100 KB/s', value: 100},
                    {text: '200 KB/s', value: 200},
                    {text: '300 KB/s', value: 300}
                ]
            }
        ]
    });
    Ext.Viewport.add(picker);
    picker.show();
 */
Ext.define('Ext.picker.Picker', {
    extend: 'Ext.Sheet',
    alias : 'widget.picker',
    alternateClassName: 'Ext.Picker',
    requires: ['Ext.picker.Slot', 'Ext.Toolbar', 'Ext.data.Model'],

    isPicker: true,

    /**
     * @event pick
     * Fired when a slot has been picked
     * @param {Ext.Picker} this This Picker
     * @param {Object} The values of this picker's slots, in {name:'value'} format
     * @param {Ext.Picker.Slot} slot An instance of Ext.Picker.Slot that has been picked
     */

    /**
     * @event change
     * Fired when the value of this picker has changed the Done button has been pressed.
     * @param {Ext.picker.Picker} this This Picker
     * @param {Object} value The values of this picker's slots, in {name:'value'} format
     */

    /**
     * @event cancel
     * Fired when the cancel button is tapped and the values are reverted back to
     * what they were
     * @param {Ext.Picker} this This Picker
     */

    config: {
        /**
         * @cfg
         * @inheritdoc
         */
        cls: Ext.baseCSSPrefix + 'picker',

        /**
         * @cfg {String/Mixed} doneButton
         * Can be either:<ul>
         * <li>A {String} text to be used on the Done button</li>
         * <li>An {Object} as config for {@link Ext.Button}</li>
         * <li>false or null to hide it</li></ul>
         * @accessor
         */
        doneButton: true,

        /**
         * @cfg {String/Mixed} cancelButton
         * Can be either:<ul>
         * <li>A {String} text to be used on the Cancel button</li>
         * <li>An {Object} as config for {@link Ext.Button}</li>
         * <li>false or null to hide it</li></ul>
         * @accessor
         */
        cancelButton: true,

        /**
         * @cfg {Boolean} useTitles
         * Generate a title header for each individual slot and use
         * the title configuration of the slot.
         * @accessor
         */
        useTitles: false,

        /**
         * @cfg {Array} slots
         * An array of slot configurations.
         * <ul>
         *  <li>name - {String} - Name of the slot</li>
         *  <li>data - {Array} - An array of text/value pairs in the format {text: 'myKey', value: 'myValue'}</li>
         *  <li>title - {String} - Title of the slot. This is used in conjunction with useTitles: true.</li>
         * </ul>
         * @accessor
         */
        slots: null,

        /**
         * @cfg {String/Number} value The value to initialize the picker with
         * @accessor
         */
        value: null,

        /**
         * @cfg {Number} height
         * The height of the picker.
         * @accessor
         */
        height: 220,

        /**
         * @cfg
         * @inheritdoc
         */
        layout: {
            type : 'hbox',
            align: 'stretch'
        },

        /**
         * @cfg
         * @hide
         */
        centered: false,

        /**
         * @cfg
         * @inheritdoc
         */
        left : 0,

        /**
         * @cfg
         * @inheritdoc
         */
        right: 0,

        /**
         * @cfg
         * @inheritdoc
         */
        bottom: 0,

        // @private
        defaultType: 'pickerslot',

        /**
         * @cfg {Ext.TitleBar/Ext.Toolbar/Object} toolbar
         * The toolbar which contains the {@link #doneButton} and {@link #cancelButton} buttons.
         * You can override this if you wish, and add your own configurations. Just ensure that you take into account
         * the {@link #doneButton} and {@link #cancelButton} configurations.
         *
         * The default xtype is a {@link Ext.TitleBar}:
         *
         *     toolbar: {
         *         items: [
         *             {
         *                 xtype: 'button',
         *                 text: 'Left',
         *                 align: 'left'
         *             },
         *             {
         *                 xtype: 'button',
         *                 text: 'Right',
         *                 align: 'left'
         *             }
         *         ]
         *     }
         *
         * Or to use a {@link Ext.Toolbar instead}:
         *
         *     toolbar: {
         *         xtype: 'toolbar',
         *         items: [
         *             {
         *                 xtype: 'button',
         *                 text: 'Left'
         *             },
         *             {
         *                 xtype: 'button',
         *                 text: 'Left Two'
         *             }
         *         ]
         *     }
         *
         * @accessor
         */
        toolbar: true
    },

    initElement: function() {
        this.callParent(arguments);

        var me = this,
            clsPrefix = Ext.baseCSSPrefix,
            innerElement = this.innerElement;

        //insert the mask, and the picker bar
        this.mask = innerElement.createChild({
            cls: clsPrefix + 'picker-mask'
        });

        this.bar = this.mask.createChild({
            cls: clsPrefix + 'picker-bar'
        });

        me.on({
            scope   : this,
            delegate: 'pickerslot',
            slotpick: 'onSlotPick'
        });

        me.on({
            scope: this,
            show: 'onShow'
        });
    },

    /**
     * @private
     */
    applyToolbar: function(config) {
        if (config === true) {
            config = {};
        }

        Ext.applyIf(config, {
            docked: 'top'
        });

        return Ext.factory(config, 'Ext.TitleBar', this.getToolbar());
    },

    /**
     * @private
     */
    updateToolbar: function(newToolbar, oldToolbar) {
        if (newToolbar) {
            this.add(newToolbar);
        }

        if (oldToolbar) {
            this.remove(oldToolbar);
        }
    },

    /**
     * Updates the {@link #doneButton} configuration. Will change it into a button when appropriate, or just update the text if needed.
     */
    applyDoneButton: function(config) {
        if (config) {
            if (Ext.isBoolean(config)) {
                config = {};
            }

            if (typeof config == "string") {
                config = {
                    text: config
                };
            }

            Ext.applyIf(config, {
                ui: 'action',
                align: 'right',
                text: 'Done'
            });
        }

        return Ext.factory(config, 'Ext.Button', this.getDoneButton());
    },

    updateDoneButton: function(newDoneButton, oldDoneButton) {
        var toolbar = this.getToolbar();

        if (newDoneButton) {
            toolbar.add(newDoneButton);
            newDoneButton.on('tap', this.onDoneButtonTap, this);
        } else if (oldDoneButton) {
            toolbar.remove(oldDoneButton);
        }
    },

    /**
     * Updates the {@link #cancelButton} configuration. Will change it into a button when appropriate, or just update the text if needed.
     */
    applyCancelButton: function(config) {
        if (config) {
            if (Ext.isBoolean(config)) {
                config = {};
            }

            if (typeof config == "string") {
                config = {
                    text: config
                };
            }

            Ext.applyIf(config, {
                align: 'left',
                text: 'Cancel'
            });
        }

        return Ext.factory(config, 'Ext.Button', this.getCancelButton());
    },

    updateCancelButton: function(newCancelButton, oldCancelButton) {
        var toolbar = this.getToolbar();

        if (newCancelButton) {
            toolbar.add(newCancelButton);
            newCancelButton.on('tap', this.onCancelButtonTap, this);
        } else if (oldCancelButton) {
            toolbar.remove(oldCancelButton);
        }
    },

    /**
     * @private
     */
    updateUseTitles: function(useTitles) {
        var innerItems = this.getInnerItems(),
            ln = innerItems.length,
            cls = Ext.baseCSSPrefix + 'use-titles',
            i, innerItem;

        //add a cls onto the picker
        if (useTitles) {
            this.addCls(cls);
        } else {
            this.removeCls(cls);
        }

        //show the titme on each of the slots
        for (i = 0; i < ln; i++) {
            innerItem = innerItems[i];

            if (innerItem.isSlot) {
                innerItem.setShowTitle(useTitles);
            }
        }
    },

    applySlots: function(slots) {
        //loop through each of the slots and add a referece to this picker
        if (slots) {
            var ln = slots.length,
                i;

            for (i = 0; i < ln; i++) {
                slots[i].picker = this;
            }
        }

        return slots;
    },

    /**
     * Adds any new {@link #slots} to this picker, and removes existing {@link #slots}
     * @private
     */
    updateSlots: function(newSlots) {
        var bcss = Ext.baseCSSPrefix,
            innerItems;

        this.removeAll();

        if (newSlots) {
            this.add(newSlots);
        }

        innerItems = this.getInnerItems();
        if (innerItems.length > 0) {
            innerItems[0].addCls(bcss + 'first');
            innerItems[innerItems.length - 1].addCls(bcss + 'last');
        }

        this.updateUseTitles(this.getUseTitles());
    },

    /**
     * @private
     * Called when the done button has been tapped.
     */
    onDoneButtonTap: function() {
        var oldValue = this._value,
            newValue = this.getValue();

        if (newValue != oldValue) {
            this.fireEvent('change', this, newValue);
        }

        this.hide();
    },

    /**
     * @private
     * Called when the cancel button has been tapped.
     */
    onCancelButtonTap: function() {
        this.fireEvent('cancel', this);
        this.hide();
    },

    /**
     * @private
     * Called when a slot has been picked.
     */
    onSlotPick: function(slot, value, node) {
        this.fireEvent('pick', this, this.getValue(), slot);
    },

    onShow: function() {
        if (!this.isHidden()) {
            this.setValue(this._value);
        }
    },

    /**
     * Sets the values of the pickers slots
     * @param {Object} values The values in a {name:'value'} format
     * @param {Boolean} animated True to animate setting the values
     * @return {Ext.Picker} this This picker
     */
    setValue: function(values, animated) {
        var me = this,
            slots = me.getInnerItems(),
            ln = slots.length,
            key, slot, loopSlot, i;

        if (!values) {
            values = {};
            for (i = 0; i < ln; i++) {
                //set the value to false so the slot will return null when getValue is set
                values[slots[i].config.name] = false;
            }
        }

        for (key in values) {
            value = values[key];
            for (i = 0; i < slots.length; i++) {
                loopSlot = slots[i];
                if (loopSlot.config.name == key) {
                    slot = loopSlot;
                    break;
                }
            }

            if (slot) {
                if (animated) {
                    slot.setValueAnimated(value);
                } else {
                    slot.setValue(value);
                }
            }
        }

        me._value = this.getValue();
        me._values = me._value;

        return me;
    },

    setValueAnimated: function(values) {
        this.setValue(values, true);
    },

    /**
     * Returns the values of each of the pickers slots
     * @return {Object} The values of the pickers slots
     */
    getValue: function() {
        var values = {},
            items = this.getItems().items,
            ln = items.length,
            item, i;

        for (i = 0; i < ln; i++) {
            item = items[i];
            if (item && item.isSlot) {
                values[item.getName()] = item.getValue();
            }
        }

        this._values = values;

        return this._values;
    },

    /**
     * Returns the values of eaach of the pickers slots
     * @return {Object} The values of the pickers slots
     */
    getValues: function() {
        return this.getValue();
    },

    destroy: function() {
        this.callParent();
        Ext.destroy(this.mask, this.bar);
    }
}, function() {
    //<deprecated product=touch since=2.0>
    /**
     * @member Ext.picker.Picker
     * @cfg {String} activeCls
     * CSS class to be applied to individual list items when they have been chosen.
     * @removed 2.0.0
     */
    Ext.deprecateProperty(this, 'activeCls', null, "Ext.picker.Picker.activeCls has been removed");

    /**
     * @method getCard
     * @inheritdoc Ext.picker.Picker#getActiveItem
     * @deprecated 2.0.0 Please use {@link #getActiveItem} instead
     */
    Ext.deprecateClassMethod(this, 'getCard', 'getActiveItem');

    /**
     * @method setCard
     * @inheritdoc Ext.picker.Picker#setActiveItem
     * @deprecated 2.0.0 Please use {@link #setActiveItem} instead
     */
    Ext.deprecateClassMethod(this, 'setCard', 'setActiveItem');

    //</deprecated>

    Ext.define('x-textvalue', {
        extend: 'Ext.data.Model',
        config: {
            fields: ['text', 'value']
        }
    });
});

/**
 * {@link Ext.ActionSheet ActionSheets} are used to display a list of {@link Ext.Button buttons} in a popup dialog.
 *
 * The key difference between ActionSheet and {@link Ext.Sheet} is that ActionSheets are docked at the bottom of the
 * screen, and the {@link #defaultType} is set to {@link Ext.Button button}.
 *
 * ## Example
 *
 *     @example preview miniphone
 *     var actionSheet = Ext.create('Ext.ActionSheet', {
 *         items: [
 *             {
 *                 text: 'Delete draft',
 *                 ui  : 'decline'
 *             },
 *             {
 *                 text: 'Save draft'
 *             },
 *             {
 *                 text: 'Cancel',
 *                 ui  : 'confirm'
 *             }
 *         ]
 *     });
 *
 *     Ext.Viewport.add(actionSheet);
 *     actionSheet.show();
 *
 * As you can see from the code above, you no longer have to specify a `xtype` when creating buttons within a {@link Ext.ActionSheet ActionSheet},
 * because the {@link #defaultType} is set to {@link Ext.Button button}.
 *
 */
Ext.define('Ext.ActionSheet', {
    extend: 'Ext.Sheet',
    alias : 'widget.actionsheet',
    requires: ['Ext.Button'],

    config: {
        /**
         * @cfg
         * @inheritdoc
         */
        baseCls: Ext.baseCSSPrefix + 'sheet-action',

        /**
         * @cfg
         * @inheritdoc
         */
        left: 0,

        /**
         * @cfg
         * @inheritdoc
         */
        right: 0,

        /**
         * @cfg
         * @inheritdoc
         */
        bottom: 0,

        // @hide
        centered: false,

        /**
         * @cfg
         * @inheritdoc
         */
        height: 'auto',

        /**
         * @cfg
         * @inheritdoc
         */
        defaultType: 'button'
    }
});
/**
 * {@link Ext.TitleBar}'s are most commonly used as a docked item within an {@link Ext.Container}.
 *
 * The main difference between a {@link Ext.TitleBar} and an {@link Ext.Toolbar} is that
 * the {@link #title} configuration is **always** centered horiztonally in a {@link Ext.TitleBar} between
 * any items aligned left or right.
 *
 * You can also give items of a {@link Ext.TitleBar} an `align` configuration of `left` or `right`
 * which will dock them to the `left` or `right` of the bar.
 *
 * ## Examples
 *
 *     @example preview
 *     Ext.Viewport.add({
 *         xtype: 'titlebar',
 *         docked: 'top',
 *         title: 'Navigation',
 *         items: [
 *             {
 *                 iconCls: 'add',
 *                 iconMask: true,
 *                 align: 'left'
 *             },
 *             {
 *                 iconCls: 'home',
 *                 iconMask: true,
 *                 align: 'right'
 *             }
 *         ]
 *     });
 *
 *     Ext.Viewport.setStyleHtmlContent(true);
 *     Ext.Viewport.setHtml('This shows the title being centered and buttons using align <i>left</i> and <i>right</i>.');
 *
 * <br />
 *
 *     @example preview
 *     Ext.Viewport.add({
 *         xtype: 'titlebar',
 *         docked: 'top',
 *         title: 'Navigation',
 *         items: [
 *             {
 *                 align: 'left',
 *                 text: 'This button has a super long title'
 *             },
 *             {
 *                 iconCls: 'home',
 *                 iconMask: true,
 *                 align: 'right'
 *             }
 *         ]
 *     });
 *
 *     Ext.Viewport.setStyleHtmlContent(true);
 *     Ext.Viewport.setHtml('This shows how the title is automatically moved to the right when one of the aligned buttons is very wide.');
 *
 * <br />
 *
 *     @example preview
 *     Ext.Viewport.add({
 *         xtype: 'titlebar',
 *         docked: 'top',
 *         title: 'A very long title',
 *         items: [
 *             {
 *                 align: 'left',
 *                 text: 'This button has a super long title'
 *             },
 *             {
 *                 align: 'right',
 *                 text: 'Another button'
 *             },
 *         ]
 *     });
 *
 *     Ext.Viewport.setStyleHtmlContent(true);
 *     Ext.Viewport.setHtml('This shows how the title and buttons will automatically adjust their size when the width of the items are too wide..');
 *
 * The {@link #defaultType} of Toolbar's is {@link Ext.Button button}.
 */
Ext.define('Ext.TitleBar', {
    extend: 'Ext.Container',
    xtype: 'titlebar',

    requires: [
        'Ext.Button',
        'Ext.Title',
        'Ext.Spacer',
        'Ext.util.SizeMonitor'
    ],

    // private
    isToolbar: true,

    config: {
        /**
         * @cfg
         * @inheritdoc
         */
        baseCls: Ext.baseCSSPrefix + 'toolbar',

        /**
         * @cfg
         * @inheritdoc
         */
        cls: Ext.baseCSSPrefix + 'navigation-bar',

        /**
         * @cfg {String} ui
         * Style options for Toolbar. Either 'light' or 'dark'.
         * @accessor
         */
        ui: 'dark',

        /**
         * @cfg {String} title
         * The title of the toolbar.
         * @accessor
         */
        title: null,

        /**
         * @cfg {String} defaultType
         * The default xtype to create.
         * @accessor
         */
        defaultType: 'button',

        /**
         * @cfg
         * @hide
         */
        layout: {
            type: 'hbox'
        },

        /**
         * @cfg {Array/Object} items The child items to add to this TitleBar. The {@link #defaultType} of
         * a TitleBar is {@link Ext.Button}, so you do not need to specify an `xtype` if you are adding
         * buttons.
         *
         * You can also give items a `align` configuration which will align the item to the `left` or `right` of
         * the TitleBar.
         * @accessor
         */
        items: []
    },

    /**
     * The max button width in this toolbar
     * @private
     */
    maxButtonWidth: '40%',

    constructor: function() {
        this.refreshTitlePosition = Ext.Function.createThrottled(this.refreshTitlePosition, 50, this);

        this.callParent(arguments);
    },

    beforeInitialize: function() {
        this.applyItems = this.applyInitialItems;
    },

    initialize: function() {
        delete this.applyItems;

        this.doAdd = this.doBoxAdd;
        this.remove = this.doBoxRemove;
        this.doInsert = this.doBoxInsert;

        this.add(this.initialItems);
        delete this.initialItems;

        this.on({
            painted: 'onPainted',
            erased: 'onErased'
        });
    },

    applyInitialItems: function(items) {
        var SizeMonitor = Ext.util.SizeMonitor,
            defaults = this.getDefaults() || {},
            leftBox, rightBox, spacer;

        this.initialItems = items;

        this.leftBox = leftBox = this.add({
            xtype: 'container',
            style: 'position: relative',
            layout: {
                type: 'hbox',
                align: 'center'
            }
        });

        this.spacer = spacer = this.add({
            xtype: 'component',
            style: 'position: relative',
            flex: 1
        });

        this.rightBox = rightBox = this.add({
            xtype: 'container',
            style: 'position: relative',
            layout: {
                type: 'hbox',
                align: 'center'
            }
        });

        this.titleComponent = this.add({
            xtype: 'title',
            hidden: defaults.hidden,
            centered: true
        });

        this.sizeMonitors = {
            leftBox: new SizeMonitor({
                element: leftBox.renderElement,
                callback: this.refreshTitlePosition,
                scope: this
            }),
            spacer: new SizeMonitor({
                element: spacer.renderElement,
                callback: this.refreshTitlePosition,
                scope: this
            }),
            rightBox: new SizeMonitor({
                element: rightBox.renderElement,
                callback: this.refreshTitlePosition,
                scope: this
            })
        };
    },

    doBoxAdd: function(item) {
        if (item.config.align == 'right') {
            this.rightBox.add(item);
        }
        else {
            this.leftBox.add(item);
        }

        if (this.painted) {
            this.refreshTitlePosition();
        }
    },

    doBoxRemove: function(item) {
        if (item.config.align == 'right') {
            this.rightBox.remove(item);
        }
        else {
            this.leftBox.remove(item);
        }

        if (this.painted) {
            this.refreshTitlePosition();
        }
    },

    doBoxInsert: function(index, item) {
        if (item.config.align == 'right') {
            this.rightBox.add(item);
        }
        else {
            this.leftBox.add(item);
        }
    },

    onPainted: function() {
        var sizeMonitors = this.sizeMonitors;

        this.painted = true;
        this.refreshTitlePosition();

        sizeMonitors.leftBox.refresh();
        sizeMonitors.spacer.refresh();
        sizeMonitors.rightBox.refresh();
    },

    onErased: function() {
        this.painted = false;
    },

    getMaxButtonWidth: function() {
        var value = this.maxButtonWidth;

        //check if it is a percentage
        if (Ext.isString(this.maxButtonWidth)) {
            value = parseInt(value.replace('%', ''), 10);
            value = Math.round((this.element.getWidth() / 100) * value);
        }

        return value;
    },

    refreshTitlePosition: function() {
        var titleElement = this.titleComponent.renderElement;

        titleElement.setWidth(null);
        titleElement.setLeft(null);

        //set the min/max width of the left button
        var leftBox = this.leftBox,
            leftButton = leftBox.down('button'),
            leftBoxWidth, maxButtonWidth;

        if (leftButton) {
            if (leftButton.getWidth() == null) {
                leftButton.renderElement.setWidth('auto');
            }

            leftBoxWidth = leftBox.renderElement.getWidth();
            maxButtonWidth = this.getMaxButtonWidth();

            if (leftBoxWidth > maxButtonWidth) {
                leftButton.renderElement.setWidth(maxButtonWidth);
            }
        }

        var spacerBox = this.spacer.renderElement.getPageBox(),
            titleBox = titleElement.getPageBox(),
            widthDiff = titleBox.width - spacerBox.width,
            titleLeft = titleBox.left,
            titleRight = titleBox.right,
            halfWidthDiff, leftDiff, rightDiff;

        if (widthDiff > 0) {
            titleElement.setWidth(spacerBox.width);
            halfWidthDiff = widthDiff / 2;
            titleLeft += halfWidthDiff;
            titleRight -= halfWidthDiff;
        }

        leftDiff = spacerBox.left - titleLeft;
        rightDiff = titleRight - spacerBox.right;

        if (leftDiff > 0) {
            titleElement.setLeft(leftDiff);
        }
        else if (rightDiff > 0) {
            titleElement.setLeft(-rightDiff);
        }

        titleElement.repaint();
    },

    // @private
    updateTitle: function(newTitle) {
        this.titleComponent.setTitle(newTitle);

        this.titleBox = null;

        if (this.painted) {
            this.refreshTitlePosition();
        }
    },

    destroy: function() {
        this.callParent();

        var sizeMonitors = this.sizeMonitors;

        sizeMonitors.leftBox.destroy();
        sizeMonitors.spacer.destroy();
        sizeMonitors.rightBox.destroy();
    }
});
